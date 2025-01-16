using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class Xml_Item : MonoBehaviour
{
    [Header("Obj Emp")]
    public Image img_icon;
    public Text txt_title;
    public Text txt_tip;
    public XmlNodeType type;
    public Transform tr_xml_body;
    public Transform tr_attr_body;
    public GameObject item_attr_prefab;
    public GameObject panel_attr;
    public GameObject obj_btn_add_attr;
    public GameObject obj_btn_add_node;
    public Image img_collect_icon;

    [Header("Asset Icon")]
    public Sprite sp_icon_node_text;
    public Sprite sp_icon_node_emp;
    public Sprite sp_icon_node_del;
    public Sprite sp_icon_node_attr;
    public Sprite sp_icon_node_list_attr;
    public Sprite sp_icon_node_edit;
    public Sprite sp_icon_collect_true;
    public Sprite sp_icon_collect_false;

    public int x=1;
    public int y=0;
    private bool is_collect=false;

    public GameObject obj_btn_delete;
    public GameObject obj_btn_collect;

    private string s_note;

    private List<Xml_Item> list_item_child;
    private List<Item_Attr> list_item_attr;


    public void On_load(XmlNodeType type_node,Xml_Item xml_item_father)
    {
        this.list_item_child = new List<Xml_Item>();
        this.list_item_attr = new List<Item_Attr>();

        this.type = type_node;
        this.obj_btn_delete.SetActive(false);
        if (this.type !=XmlNodeType.None) this.obj_btn_delete.SetActive(true);

        if (this.type ==XmlNodeType.Text)
        {
            this.img_icon.sprite = this.sp_icon_node_text;
            this.img_icon.color= new Color32(255, 119, 0, 255);
            this.txt_title.color = new Color32(255, 119, 0, 255);
            this.txt_tip.color = new Color32(255, 154, 0, 225);
            this.obj_btn_add_node.SetActive(false);
            this.obj_btn_add_attr.SetActive(false);
            this.obj_btn_collect.SetActive(false);
        }

        if (xml_item_father != null)
        {
            this.y = xml_item_father.y+ 1;
            this.x = xml_item_father.x + 1;
            xml_item_father.add_child(this);
        }
        else
        {
            this.x = 0;
            this.y = 0;
        }

        RectTransform r = this.tr_xml_body.GetComponent<RectTransform>();
        r.offsetMin = new Vector2(this.x*20f, r.offsetMin.y);
        r.offsetMax = new Vector2(0f, r.offsetMax.y);
        r.offsetMax = new Vector2(r.offsetMax.x, 0f);
        r.offsetMin = new Vector2(r.offsetMin.x, 0f);

        this.transform.SetSiblingIndex(this.y);
        this.panel_attr.SetActive(false);
        this.check_collect();
    }

    public void add_child(Xml_Item xml_Item)
    {
        this.list_item_child.Add(xml_Item);
    }

    public void btn_add_attr()
    {
        GameObject.Find("App").GetComponent<Apps>().xml.show_box_add_attr(this);
    }

    public void btn_add_note()
    {
        GameObject.Find("App").GetComponent<Apps>().xml.show_box_add_note(this);
    }

    public void set_title(string s_title)
    {
        this.txt_title.text = s_title;
        this.s_note = s_title;
    }

    public void set_tip(string s_tip)
    {
        this.txt_tip.text = s_tip;
    }

    private string get_space()
    {
        string s_space = "";
        for(int i = 0; i < this.x; i++) s_space += "\t";
        return s_space;
    }

    public string get_code_full()
    {
        string s_data;
        if (this.type == XmlNodeType.Text)
        {
            s_data = this.get_space()+this.s_note+"\n";
        }
        else
        {
            s_data = this.get_space() + "<" + this.s_note + this.get_s_attr() + ">\n";
            for (int i = 0; i < this.list_item_child.Count; i++) if(this.list_item_child[i]!=null) s_data += this.list_item_child[i].get_code_full();
            s_data = s_data + this.get_space() + "</" + this.s_note + ">\n";

        }
        return s_data;
    }

    public string get_code_short()
    {
        string s_data;
        if (this.type == XmlNodeType.Text)
        {
            s_data = this.s_note;
        }
        else
        {
            s_data = "<" + this.s_note + this.get_s_attr() + ">";
            for (int i = 0; i < this.list_item_child.Count; i++) if (this.list_item_child[i] != null) s_data += this.list_item_child[i].get_code_short();
            s_data += "</" + this.s_note + ">";
        }

        return s_data;
    }

    private string get_s_attr()
    {
        string s_attr=" ";
        for(int i = 0; i < this.list_item_attr.Count; i++)
        {
            s_attr= s_attr+this.list_item_attr[i].get_s_name()+"=\""+this.list_item_attr[i].get_s_value()+"\" ";
        }

        if (this.list_item_attr.Count > 0)
            return s_attr;
        else
            return "";
    }

    public List<Xml_Item> get_list_child()
    {
        return this.list_item_child;
    }

    public void Delete_all_child()
    {
        for (int i = 0; i < list_item_child.Count; i++)
        {
            if (this.list_item_child[i] != null)
            {
                this.list_item_child[i].Delete_all_child();
                Destroy(this.list_item_child[i].gameObject);
            }
        }
        this.list_item_child.Clear();
        this.list_item_child= null;
    }

    public void btn_delete()
    {
        GameObject.Find("App").GetComponent<Apps>().xml.Delete_node(this);
    }

    public void add_attr_item(string s_name,string s_val)
    {
        GameObject item_attr = Instantiate(this.item_attr_prefab);
        item_attr.transform.SetParent(this.tr_attr_body);
        item_attr.transform.localPosition = Vector3.zero;
        item_attr.transform.localScale = new Vector3(1f, 1f, 1f);

        Item_Attr attr = item_attr.GetComponent<Item_Attr>();
        attr.on_load(s_name, s_val);
        this.list_item_attr.Add(attr);

        this.panel_attr.SetActive(true);
    }

    public void btn_open_menu()
    {
        GameObject.Find("App").GetComponent<Apps>().xml.show_menu_node(this);
    }

    public string get_s_nodes()
    {
        return this.s_note;
    }

    public void btn_collect()
    {
        if (this.is_collect)
        {
            this.is_collect = false;
            this.collect_all_child(true);
        }
        else
        {
            this.is_collect = true;
            this.collect_all_child(false);
        }
            
        this.check_collect();
    }

    private void check_collect()
    {
        if (this.is_collect)
            this.img_collect_icon.sprite = this.sp_icon_collect_true;
        else
            this.img_collect_icon.sprite = this.sp_icon_collect_false;
    }

    public void collect_all_child(bool is_c)
    {
        for(int i = 0; i < this.list_item_child.Count; i++)
        {
            this.list_item_child[i].collect_all_child(is_c);
            this.list_item_child[i].gameObject.SetActive(is_c);
        }
    }

    public List<Item_Attr> get_list_item_attr()
    {
        return this.list_item_attr;
    }

    public void Delete_attr(int index)
    {
        Debug.Log("Attr count:" + this.list_item_attr.Count);
        if (this.list_item_attr[index] != null)
        {
            Destroy(this.list_item_attr[index].gameObject);
            this.list_item_attr[index] = null;
        }

        List<Item_Attr> list_new = new List<Item_Attr>();
        
        for(int i=0;i<list_item_attr.Count;i++)
        {
            if(list_item_attr[i]!=null)list_new.Add(list_item_attr[i]);
        }
        this.list_item_attr = list_new;
        Debug.Log("Attr count:"+this.list_item_attr.Count);
        if(this.list_item_attr.Count==0) this.panel_attr.SetActive(false);
    }
}
