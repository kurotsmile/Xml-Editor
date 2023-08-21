using System;
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

    private Carrot.Carrot_Window_Msg box_msg;
    private Carrot.Carrot_Box box_menu;

    public void on_load(XmlNodeType type_node,Xml_Item xml_item_father)
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
            this.txt_tip.text = "As text object";
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
        if (this.box_menu != null) this.box_menu.close();
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

    private string get_space()
    {
        string s_space = "";
        for(int i = 0; i < this.x; i++) s_space = s_space + "\t";
        return s_space;
    }

    public string get_code_full()
    {
        string s_data;
        if (this.type == XmlNodeType.Text)
        {
            s_data =this.s_note;
        }
        else
        {
            if (this.list_item_child.Count==1)
            {
                Xml_Item xml_nodes = this.list_item_child[0];
                if(xml_nodes.type== XmlNodeType.Text)
                    s_data = this.get_space() + "<" + this.s_note + this.get_s_attr() + ">"+ xml_nodes.s_note+ "</" + this.s_note + ">\n";
                else
                {
                    s_data = this.get_space() + "<" + this.s_note + this.get_s_attr() + ">\n";
                    for (int i = 0; i < this.list_item_child.Count; i++) s_data = s_data + this.list_item_child[i].get_code_full();
                    s_data = s_data + this.get_space() + "</" + this.s_note + ">\n";
                }
            }
            else
            {
                s_data = this.get_space() + "<" + this.s_note + this.get_s_attr() + ">\n";
                for (int i = 0; i < this.list_item_child.Count; i++) s_data = s_data + this.list_item_child[i].get_code_full();
                s_data = s_data + this.get_space() + "</" + this.s_note + ">\n";
            }

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
            if (this.list_item_child.Count == 1)
            {
                Xml_Item xml_nodes = this.list_item_child[0];
                if (xml_nodes.type == XmlNodeType.Text )
                    s_data ="<" + this.s_note + this.get_s_attr() + ">" + xml_nodes.s_note + "</" + this.s_note + ">";
                else
                {
                    s_data ="<" + this.s_note + this.get_s_attr() + ">";
                    for (int i = 0; i < this.list_item_child.Count; i++) s_data = s_data + this.list_item_child[i].get_code_short();
                    s_data = s_data + "</" + this.s_note + ">";
                }
            }
            else
            {
                s_data = "<" + this.s_note + this.get_s_attr() + ">";
                for (int i = 0; i < this.list_item_child.Count; i++)s_data = s_data + this.list_item_child[i].get_code_short();
                s_data = s_data + "</" + this.s_note + ">";
            }
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

    public void btn_delete()
    {
        this.box_msg = GameObject.Find("App").GetComponent<Apps>().carrot.show_msg("Xml Node", "Are you sure you want to remove this item?", Carrot.Msg_Icon.Question);
        this.box_msg.add_btn_msg("Yes", act_delete_yes);
        this.box_msg.add_btn_msg("No", act_delete_no);
    }

    private void act_delete_yes()
    {
        if (this.box_menu != null) this.box_menu.close();
        for (int i = 0; i < this.list_item_child.Count; i++) Destroy(this.list_item_child[i].gameObject);
        Destroy(this.gameObject);
        this.box_msg.close();
    }

    private void act_delete_no()
    {
        this.box_msg.close();
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
        this.box_menu = GameObject.Find("App").GetComponent<Apps>().carrot.Create_Box("box_menu");
        box_menu.set_title("Menu - ("+this.get_s_nodes()+")");

        Carrot.Carrot_Box_Item item_edit = box_menu.create_item("i_edit");
        item_edit.set_icon(this.sp_icon_node_edit);
        item_edit.set_title("Edit");
        item_edit.set_tip("Edit node data");
        item_edit.set_act(btn_edit);

        if (this.type != XmlNodeType.Text)
        {
            Carrot.Carrot_Box_Item item_attr = box_menu.create_item("i_attr");
            item_attr.set_icon(this.sp_icon_node_attr);
            item_attr.set_title("Add attributes");
            item_attr.set_tip("Add properties for node");
            item_attr.set_act(this.btn_add_attr);

            if (this.list_item_attr.Count > 0)
            {
                Carrot.Carrot_Box_Item item_list_attr = box_menu.create_item("i_list_attr");
                item_list_attr.set_icon(this.sp_icon_node_list_attr);
                item_list_attr.set_title("List attributes");
                item_list_attr.set_tip("Displays a list of node's attributes");
                item_list_attr.set_act(this.btn_show_list_attr);
            }
        }

        Carrot.Carrot_Box_Item item_del=box_menu.create_item("i_delete");
        item_del.set_icon(this.sp_icon_node_del);
        item_del.set_title("Delete");
        item_del.set_tip("Delete node");
        item_del.set_act(this.btn_delete);
    }

    private void btn_edit()
    {
        if (this.box_menu != null) this.box_menu.close();
        GameObject.Find("App").GetComponent<Apps>().xml.box_add.show_edit(this);
    }

    private void btn_show_list_attr()
    {
        if (this.box_menu != null) this.box_menu.close();

        this.box_menu = GameObject.Find("App").GetComponent<Apps>().carrot.Create_Box("box_attr");
        this.box_menu.set_icon(this.sp_icon_node_list_attr);
        this.box_menu.set_title("Attribute list of node");

        for (int i = 0; i < this.list_item_attr.Count; i++)
        {
            Carrot.Carrot_Box_Item item_attr=this.box_menu.create_item("item_attr_" + i);
            item_attr.set_icon(this.sp_icon_node_attr);
            item_attr.set_title(this.list_item_attr[i].get_s_name());
            item_attr.set_tip(this.list_item_attr[i].get_s_value());

            var attr_set = this.list_item_attr[i];
            item_attr.set_act(() => show_edit_attr(attr_set));
        }
    }

    private void show_edit_attr(Item_Attr attr)
    {
        if (this.box_menu != null) this.box_menu.close();

        GameObject.Find("App").GetComponent<Apps>().xml.box_add.show_edit_attr(attr);
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
}
