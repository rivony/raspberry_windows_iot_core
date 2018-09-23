#include <sstream>
#include <iostream>
#include <ppltasks.h>

using namespace Platform;
using namespace Windows::Devices::I2c;

class wexception
{
public:
	explicit wexception(const std::wstring &msg) : msg_(msg) { }
	virtual ~wexception() { /*empty*/ }

	virtual const wchar_t *wwhat() const
	{
		return msg_.c_str();
	}

private:
	std::wstring msg_;
};

I2cDevice^ MakeDevice(int slaveAddress, _In_opt_ String^ friendlyName)
{
  using namespace Windows::Devices::Enumeration;

  String^ aqs;
  if (friendlyName)
    aqs = I2cDevice::GetDeviceSelector(friendlyName);
  else
    aqs = I2cDevice::GetDeviceSelector();

  auto dis = concurrency::create_task(DeviceInformation::FindAllAsync(aqs)).get();
  if (dis->Size < 1)
  {
    throw wexception(L"I2C controller not found");
  }

  String^ id = dis->GetAt(0)->Id;
  auto device = concurrency::create_task(I2cDevice::FromIdAsync(id, ref new I2cConnectionSettings(slaveAddress))).get();

  if (!device)
  {
    std::wostringstream msg;
    msg << L"Slave address 0x" << std::hex << slaveAddress << L" on bus " << id->Data() << L" is in use. Please ensure that no other applications are using I2C.";
    throw wexception(msg.str());
  }
  return device;
}

std::wostream& operator<< (std::wostream& os, const Platform::Array<BYTE>^ bytes)
{
  for (auto byte : bytes)
    os << L" " << std::hex << byte;
  return os;
}

WORD BcdToInt(byte value)
{
  return ((value / 16 * 10) + (value % 16));
}

void SetTime(byte year, byte month, byte day, byte hour, byte minute, byte second)
{
  SYSTEMTIME time;
  time.wYear  = BcdToInt(year) + 2000;
  time.wMonth  = BcdToInt(month);
  time.wDay    = BcdToInt(day);
  time.wHour   = BcdToInt(hour);
  time.wMinute = BcdToInt(minute);
  time.wSecond = BcdToInt(second);

  time.wMilliseconds = 0;
  time.wDayOfWeek = 0;

  std::wcout << time.wYear << L" " << time.wMonth << L" " << time.wDay << L" " << time.wHour << L" " << time.wMinute << L" " << time.wSecond << L" " << time.wMilliseconds << L"\n";

  if (SetSystemTime(&time))
    std::wcout << L"System Time set !\n";
  else
  {
    LPVOID lpMsgBuf;
    DWORD dw = GetLastError();

    FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
      NULL,
      dw,
      MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
      (LPTSTR)&lpMsgBuf,
      0,
      NULL);

    std::wcout << L"System Time NOT set !!!!!: Error =" << GetLastError() << L" = " << lpMsgBuf << L"\n";
  }
}

void AskTime(I2cDevice^ device)
{
  std::vector<BYTE> writeBuf;
  writeBuf = { 00 };

  auto readBuf = ref new Array<BYTE>(7);

  I2cTransferResult result = device->WriteReadPartial(ArrayReference<BYTE>(writeBuf.data(), static_cast<unsigned int>(writeBuf.size())), readBuf);

  switch (result.Status)
  {
  case I2cTransferStatus::FullTransfer:
  {
    std::wcout << readBuf << L"\n";
    SetTime(readBuf[6],
      readBuf[5] & 0x1F,
      readBuf[4],
      readBuf[2],
      readBuf[1],
      readBuf[0] & 0x7F);
    break; 
  }
  case I2cTransferStatus::PartialTransfer:
  {
    std::wcout << L"Partial Transfer. Transferred "
      << result.BytesTransferred
      << L" bytes\n";
    int bytesRead = result.BytesTransferred - int(writeBuf.size());
    if (bytesRead > 0)
    {
      std::wcout << readBuf << L"\n";
    }
    break; 
  }
  case I2cTransferStatus::SlaveAddressNotAcknowledged:
    std::wcout << L"Slave address was not acknowledged\n";
    break;
  default:
    throw wexception(L"Invalid transfer status value");
  }
}

int main(Platform::Array<Platform::String^>^ args)
{
	int slaveAddress = 0x6F;

	String^ friendlyName;
  if (args->Length > 1)
  {
    friendlyName = args->get(1);
  }

  try 
  {
    auto device = MakeDevice(slaveAddress, friendlyName);
    AskTime(device);
  }
	catch (const wexception& ex) 
  {
		std::wcerr << L"Error: " << ex.wwhat() << L"\n";
		return 1;
	}
	catch (Platform::Exception^ ex) 
  {
		std::wcerr << L"Error: " << ex->Message->Data() << L"\n";
		return 1;
	}
	return 0;
}