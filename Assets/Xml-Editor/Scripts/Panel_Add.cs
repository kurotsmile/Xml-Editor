using System.Xml;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Panel_Add : MonoBehaviour
{
    [Header("Obj Main")]
    public Apps app;

    [Header("Asset icon")]
    public Sprite sp_icon_attr;
    public Sprite sp_icon_note;
    public Sprite sp_icon_text;

    [Header("Obj Panel")]
    public Image img_icon;
    public GameObject pane_add;
    public GameObject pane_add_name;
    public GameObject pane_add_value;

    public InputField inp_name;
    public InputField inp_val;

    public Text txt_title;
    public Text txt_tip;

    private UnityAction act_close;
    private UnityAction act_done;

    private Xml_Item xml_item_cur;
    private XmlNodeType xml_node_type;
    private Item_Attr item_attr;

    private bool is_edit = false;

    public void on_load()
    {
        this.pane_add.SetActive(false);
    }

    public void set_act_done(UnityAction act_new)
    {
        this.act_done = act_new;
    }

    public void add_act_close(UnityAction act_new)
    {
        this.act_close = act_new;
    }

    public void btn_close()
    {
        if (this.act_close != null) this.act_close();
        this.pane_add.SetActive(false);
        this.xml_item_cur = null;
    }

    public void btn_done()
    {
        if (this.act_done != null) this.act_done();
        if (this.is_edit)
        {
            if (this.xml_node_type == XmlNodeType.Text)
            {
                this.xml_item_cur.set_title(this.inp_val.text);
            }
            else
            {
                this.xml_item_cur.set_title(this.inp_name.text);
            }
        }
        this.pane_add.SetActive(false);
    }

    public void show(Xml_Item item_xml_set,XmlNodeType node_type)
    {
        this.act_done = null;
        this.act_close = null;
        this.is_edit = false;
        this.xml_node_type = node_type;
        this.update_ui_type();
        this.xml_item_cur = item_xml_set;
        this.inp_name.text = "";
        this.inp_val.text = "";
        this.pane_add.SetActive(true);
    }

    private void update_ui_type()
    {
        if (this.xml_node_type == XmlNodeType.Attribute)
        {
            this.img_icon.sprite = this.sp_icon_attr;
            this.txt_title.text = app.carrot.L("add_node_attr", "Add xml Attributes");
            this.txt_tip.text = app.carrot.L("add_node_attr_tip","Enter the attribute information");
            this.pane_add_name.SetActive(true);
            this.pane_add_value.SetActive(true);
        }

        if (this.xml_node_type == XmlNodeType.Element)
        {
            this.img_icon.sprite = this.sp_icon_note;
            this.txt_title.text = app.carrot.L("add_node","Add xml node");
            this.txt_tip.text = app.carrot.L("add_node_tip", "Enter the node block name");
            this.pane_add_name.SetActive(true);
            this.pane_add_value.SetActive(false);
        }

        if (this.xml_node_type == XmlNodeType.Text)
        {
            this.img_icon.sprite = this.sp_icon_text;
            this.txt_title.text = app.carrot.L("add_text_node","Paragraph text node");
            this.txt_tip.text = app.carrot.L("add_text_node_tip","Enter text for the object");
            this.pane_add_name.SetActive(false);
            this.pane_add_value.SetActive(true);
        }
    }

    public void show_edit(Xml_Item item_xml)
    {
        this.act_done = null;
        this.act_close = null;
        this.is_edit = true;
        this.xml_item_cur = item_xml;
        this.xml_node_type = item_xml.type;

        if (this.xml_node_type == XmlNodeType.Text)
            this.inp_val.text = item_xml.get_s_nodes();
        else
            this.inp_name.text = item_xml.get_s_nodes();

        this.update_ui_type();
        this.pane_add.SetActive(true);
    }

    public void show_edit_attr(Item_Attr item_attr)
    {
        this.item_attr = item_attr;
        this.img_icon.sprite = this.sp_icon_attr;
        this.txt_title.text = app.carrot.L("edit_attr","Edit Attributes");
        this.txt_tip.text = app.carrot.L("edit_attr","Enter the attribute information");
        this.pane_add_name.SetActive(true);
        this.pane_add_value.SetActive(true);
        this.inp_name.text = item_attr.get_s_name();
        this.inp_val.text = item_attr.get_s_value();
        this.pane_add.SetActive(true);
        this.set_act_done(act_done_edit_attr);
    }

    private void act_done_edit_attr()
    {
        this.item_attr.on_load(this.inp_name.text, this.inp_val.text);
    }

    public string get_value()
    {
        return this.inp_val.text;
    }

    public string get_name()
    {
        return this.inp_name.text;
    }

    public Xml_Item get_xml_cur()
    {
        return this.xml_item_cur;
    }

    public XmlNodeType get_type()
    {
        return this.xml_node_type;
    }
}
