using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CoursesListLIB
{
  public enum ProprieteProduitModifiee { propNom, propNb, propImg };
  public delegate void ModificationProduit(object sender, ProprieteProduitModifiee propriete);
  public class Produit
  {
    private string _nom = "";
    private uint _nb = 0;
    private async void ChargerInfosOpenFoodFacts()
    {
      HttpWebRequest webReq = WebRequest.CreateHttp("http://fr.openfoodfacts.org/api/v0/produit/" + CodeBarres + ".xml");
      WebResponse webRep = await webReq.GetResponseAsync();
      Stream data = webRep.GetResponseStream();
      XmlDocument xmlDoc = new XmlDocument();
      xmlDoc.Load(data);

      XmlElement optElem = xmlDoc.DocumentElement;
      if (optElem.GetAttribute("status") == "1")
      {
        XmlNodeList products = optElem.GetElementsByTagName("product");
        if (products.Count >= 1)
        {
          XmlNode product = products[0];
          if (Nom == "")
            Nom = GetValeurAttribut(product.Attributes, "product_name") + " " + GetValeurAttribut(product.Attributes, "quantity") + " " + GetValeurAttribut(product.Attributes, "brands");
          UrlImage = GetValeurAttribut(product.Attributes, "image_front_thumb_url");
        }
      }
      SurModificationProduit?.Invoke(this, ProprieteProduitModifiee.propImg);
    }
    private string GetValeurAttribut(XmlAttributeCollection xmlAttrCollection, string attrName)
    {
      XmlAttribute xmlAttr = xmlAttrCollection[attrName];
      return (xmlAttr == null) ? "" : xmlAttr.Value;
    }

    protected Produit(string codeBarres)
    {
      CodeBarres = codeBarres;
    }

    public string CodeBarres { get; private set; }
    public string UrlImage { get; private set; }
    public string Nom
    {
      get { return _nom; }
      set
      {
        if (value != _nom)
        {
          _nom = value;
          SurModificationProduit?.Invoke(this, ProprieteProduitModifiee.propNom);
          ListeProduits.instance.Modifiee = true;
        }
      }
    }
    public uint Nb
    {
      get { return _nb; }
      set
      {
        if (value != _nb)
        {
          _nb = value;
          SurModificationProduit?.Invoke(this, ProprieteProduitModifiee.propNb);
          ListeProduits.instance.Modifiee = true;
        }
      }
    }
    public string GetLigneCSV(char sep)
    {
      return CodeBarres + sep + Nb.ToString() + sep + Nom;
    }  

    public event ModificationProduit SurModificationProduit;

    public static Produit CreerDepuisCodeBarres(string codeBarres)
    {
      Produit res = new Produit(codeBarres);
      res.ChargerInfosOpenFoodFacts();
      return res;
    }
    public static Produit CreerDepuisLigneCsv(string ligne, char sep)
    {
      string[] elements = ligne.Split(sep);
      Produit res = new Produit(elements[0]);
      res.Nb = Convert.ToUInt32(elements[1]);
      res.Nom = elements[2];
      res.ChargerInfosOpenFoodFacts();
      return res;
    }
  }
}
