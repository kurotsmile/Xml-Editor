using Carrot;
using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Xml_Editor : MonoBehaviour
{
    [Header("Obj App")]
    public Apps app;
    public GameObject panel_editor;
    public GameObject panel_editor_code;
    public GameObject panel_editor_note;
    public GameObject xml_item_prefab;
    public Panel_Add box_add;

    [Header("Ui Emp")]
    public Image img_model_icon;
    public Text txt_name_project;
    public Text txt_model_project;
    public Transform tr_all_obj;
    public InputField inp_editor_text;
    public GameObject obj_btn_code_mode;
    public Image img_model_code_icon;
    public Text txt_model_code_text;

    [Header("Asset")]
    public Sprite sp_icon_attributes;
    public Sprite sp_icon_box_add;
    public Sprite sp_icon_model_editor;
    public Sprite sp_icon_model_code;
    public Sprite sp_icon_node_text;
    public Sprite sp_icon_box_none;
    public Sprite sp_icon_model_code_full;
    public Sprite sp_icon_model_code_short;
    public Sprite sp_icon_export_file_xml;
    public Sprite sp_icon_import_file_xml;
    public Sprite sp_icon_import_url_xml;

    private bool is_mode_editor = true;
    private bool is_mode_code_full = true;
    private int index_edit = -1;
    private Xml_Item xml_root;
    private string s_name_project;

    private Carrot.Carrot_Box box;
    private Carrot.Carrot_Box box_sub;
    private Carrot.Carrot_Window_Input box_input;
    private Carrot.Carrot_Window_Input box_input_project_url;
    private Carrot.Carrot_Window_Loading box_loading_import_xml;

    private Carrot_Box_Item item_edit_name;
    private Carrot_Box_Item item_edit_describe;

    public void on_load()
    {
        this.panel_editor.SetActive(false);
        this.box_add.on_load();
        this.check_mode_editor();
    }

    private void check_mode_editor()
    {
        this.panel_editor_note.SetActive(false);
        this.panel_editor_code.SetActive(false);

        if (this.is_mode_editor)
        {
            this.panel_editor_note.SetActive(true);
            this.txt_model_project.text = app.carrot.L("view_code","View Code");
            this.img_model_icon.sprite = this.sp_icon_model_editor;
            this.obj_btn_code_mode.SetActive(false);
        }
        else
        {
            this.panel_editor_code.SetActive(true);
            this.inp_editor_text.text = this.xml_root.get_code_full();
            this.txt_model_project.text = app.carrot.L("editor", "Data design");
            this.img_model_icon.sprite = this.sp_icon_model_code;
            this.obj_btn_code_mode.SetActive(true);
            this.check_mode_code();
        }
    }

    private void check_mode_code()
    {
        if (this.is_mode_code_full)
        {
            this.inp_editor_text.text = this.xml_root.get_code_full();
            this.img_model_code_icon.sprite = this.sp_icon_model_code_short;
            this.txt_model_code_text.text = app.carrot.L("short_code", "Shortened code");
        }
        else
        {
            this.inp_editor_text.text = this.xml_root.get_code_short();
            this.img_model_code_icon.sprite = this.sp_icon_model_code_full;
            this.txt_model_code_text.text = app.carrot.L("full_code","Nomal code");
        }
    }

    public void create_project(string s_name_project,string s_data=null,bool is_create_root=true)
    {
        this.s_name_project = s_name_project;
        this.txt_name_project.text = s_name_project;
        this.panel_editor.SetActive(true);
        this.index_edit = this.app.xml_manager.add_project(s_name_project, s_data);

        this.app.carrot.clear_contain(this.tr_all_obj);

        if (is_create_root)
        {
            this.xml_root = this.Add_note(XmlNodeType.Element, "root", null);
            this.check_mode_code();
        }
    }

    public void closer_project()
    {
        app.carrot.play_sound_click();
        this.save_project();
        this.panel_editor.SetActive(false);
    }

    public void show_box_add_note(Xml_Item item_set)
    {
        this.box=this.app.carrot.Create_Box("box_list_note");
        this.box.set_title(app.carrot.L("list_obj_add_node","List of xml node objects to add to the tree"));
        this.box.set_icon(this.sp_icon_box_add);

        Carrot.Carrot_Box_Item item_note_none = this.box.create_item();
        item_note_none.set_icon(this.sp_icon_box_none);
        item_note_none.set_title(app.carrot.L("node_none", "Empty Xml Block"));
        item_note_none.set_tip(app.carrot.L("node_none_tip","Add an empty block so that other objects can be inserted later"));
        item_note_none.set_act(() => act_add_note_to_box(item_set,XmlNodeType.Element));

        Carrot.Carrot_Box_Item item_note_value = this.box.create_item();
        item_note_value.set_icon(this.sp_icon_node_text);
        item_note_value.set_title(app.carrot.L("node_content","Note whose content is text"));
        item_note_value.set_tip(app.carrot.L("node_content_tip", "Insert a note block with echoes into the tree"));
        item_note_value.set_act(() => act_add_note_to_box(item_set,XmlNodeType.Text));
    }

    private void act_add_note_to_box(Xml_Item xml_item,XmlNodeType xml_type)
    {
        if (this.box != null) this.box.close();
        this.app.carrot.play_sound_click();
        this.box_add.show(xml_item, xml_type);
        this.box_add.set_act_done(act_add_note);
    }

    public void show_box_add_attr(Xml_Item item_set)
    {
        this.app.carrot.play_sound_click();
        this.box_add.show(item_set,XmlNodeType.Attribute);
        this.box_add.set_act_done(act_add_attr);
    }

    private void act_add_note()
    {
        this.app.carrot.play_sound_click();
        Xml_Item xml_sel = this.box_add.get_xml_cur();
        if(box_add.get_type()==XmlNodeType.Text)
            this.Add_note(box_add.get_type(), this.box_add.get_value(), xml_sel);
        else
            this.Add_note(box_add.get_type(), this.box_add.get_name(), xml_sel);
        this.box_add.btn_close();
        this.update_index_ui_emp_xml();
    }

    private void act_add_attr()
    {
        this.app.carrot.play_sound_click();
        Xml_Item xml_sel = this.box_add.get_xml_cur();
        xml_sel.add_attr_item(this.box_add.get_name(), this.box_add.get_value());
        this.box_add.btn_close();
        this.update_index_ui_emp_xml();
    }

    private void update_index_ui_emp_xml()
    {
        for(int i=0;i<this.tr_all_obj.childCount;i++)
        {
            Xml_Item tr_item = this.tr_all_obj.GetChild(i).GetComponent<Xml_Item>();
            tr_item.y = i;
        }
    }

    private Xml_Item Add_note(XmlNodeType type,string s_val,Xml_Item item_xml_father)
    {
        GameObject obj_note = Instantiate(this.xml_item_prefab);
        obj_note.transform.SetParent(this.tr_all_obj);
        obj_note.transform.localPosition = Vector3.zero;
        obj_note.transform.localScale = new Vector3(1f, 1f, 1f);
        obj_note.GetComponent<Xml_Item>().set_title(this.box_add.get_value());

        Xml_Item xml_p = obj_note.GetComponent<Xml_Item>();
        xml_p.On_load(type, item_xml_father);
        xml_p.set_title(s_val);

        if (type == XmlNodeType.Text)
            xml_p.set_tip(app.carrot.L("node_text_tip", "Is the text content object of node"));
        else
            xml_p.set_tip(app.carrot.L("node_tip", "Click here to add more properties"));

        return xml_p;
    }

    public void open_project_xml_by_index(int index)
    {
        this.index_edit = index;
        this.app.carrot.play_sound_click();
        this.index_edit = index;
        this.s_name_project = PlayerPrefs.GetString("xml_" + index + "_name");
        this.txt_name_project.text = this.s_name_project;
        this.panel_editor.SetActive(true);
        
        string s_data= PlayerPrefs.GetString("xml_" + index + "_data");
        this.ParseXML(s_data);
    }

    public void btn_delete()
    {
        this.app.carrot.play_sound_click();
        this.app.carrot.Show_msg(app.carrot.L("editor", "Xml Editor"), app.carrot.L("p_delete_question", "Are you sure you want to delete this xml project?"), () =>
        {
            this.app.xml_manager.delete_project(this.index_edit);
            this.panel_editor.SetActive(false);
        });
    }

    public void btn_change_mode_editor()
    {
        this.app.carrot.play_sound_click();
        if (this.is_mode_editor)
            this.is_mode_editor = false;
        else
            this.is_mode_editor = true;
        this.check_mode_editor();
    }

    public void btn_change_mode_code()
    {
        this.app.carrot.play_sound_click();
        if (this.is_mode_code_full)
            this.is_mode_code_full = false;
        else
            this.is_mode_code_full = true;
        this.check_mode_code();
    }

    public void Show_import()
    {
        app.carrot.ads.show_ads_Interstitial();
        this.box = app.carrot.Create_Box();
        this.box.set_icon(this.app.carrot.icon_carrot_add);
        this.box.set_title(app.carrot.L("import","Import"));

        Carrot_Box_Item item_import_file = this.box.create_item("item_import_file");
        item_import_file.set_icon(this.sp_icon_import_file_xml);
        item_import_file.set_title(app.carrot.L("import_file","Import form file"));
        item_import_file.set_tip(app.carrot.L("import_file_tip","Import project from xml file"));
        item_import_file.set_act(() => this.Act_import_from_file());

        Carrot_Box_Item item_import_url = this.box.create_item("item_import_url");
        item_import_url.set_icon(this.sp_icon_import_url_xml);
        item_import_url.set_title(app.carrot.L("import_url","Import form Url"));
        item_import_url.set_tip(app.carrot.L("import_url_tip","Import project from xml web address"));
        item_import_url.set_act(() => this.import_project_from_url());

    }

    private void Act_import_from_file()
    {
        app.carrot.play_sound_click();
        Carrot_File_Query q = new Carrot_File_Query();
        q.Add_filter("XML data", "xml", "rdf", "rss");
        q.Add_filter("XHTML data", "xhtml", "xsl", "atom");
        q.SetDefaultFilter("xml");
        app.file.Open_file(q, Act_load_file_xml_done, Act_load_file_xml_cancel);
    }

    private void Act_load_file_xml_done(string[] path)
    {
        string s_data = FileBrowserHelpers.ReadTextFromFile(path[0]);
        this.Load_import_xml(s_data);
        box?.close();
    }

    private void Act_load_file_xml_cancel()
    {
        box?.close();
    }

    private void import_project_from_url()
    {
        this.box_input_project_url = this.app.carrot.show_input(app.carrot.L("import_url", "Import form Url"),app.carrot.L("import_url_address","Enter the url xml file (eg https://www.w3schools.com/xml/note.xml)"), s_name_project);
        this.box_input_project_url.set_act_done(act_import_project_done);
    }

    private void act_import_project_done(string s_data)
    {
        if (this.box_input_project_url != null) this.box_input_project_url.close();
        this.box_loading_import_xml=this.app.carrot.show_loading(get_data_by_url(s_data));
    }

    IEnumerator get_data_by_url(string s_url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(s_url))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                this.app.carrot.Show_msg(app.carrot.L("import_url", "Import form Url"), www.error, Carrot.Msg_Icon.Error);
                this.box_loading_import_xml.close();
            }
            else
            {
                this.Load_import_xml(www.downloadHandler.text);
                this.box_loading_import_xml.close();
            }
        }
    }

    private void Load_import_xml(string s_data)
    {
        this.s_name_project = "Improt xml " + this.app.xml_manager.get_length_project();
        this.create_project(this.s_name_project, s_data, false);
        ParseXML(s_data);
    }

    public void ParseXML(string xmlString)
    {
        this.inp_editor_text.text = xmlString;
        this.app.carrot.clear_contain(this.tr_all_obj);
        XmlDocument xmlDoc = new();
        xmlDoc.LoadXml(xmlString);
        XmlNode node = xmlDoc.SelectSingleNode("*");
        parse_note_child(node,null);
    }

    private void parse_note_child(XmlNode node, Xml_Item xml_node_father)
    {
        if (node != null)
        {
            Xml_Item xml_father;

            if (xml_node_father == null)
            {
                xml_father = this.Add_note(XmlNodeType.Element, node.Name, xml_node_father);
                xml_father.obj_btn_delete.SetActive(false);
                this.xml_root = xml_father;
            }
            else
            {
                if (node.NodeType == XmlNodeType.Text)
                    xml_father = this.Add_note(node.NodeType, node.InnerText, xml_node_father);
                else
                    xml_father = this.Add_note(node.NodeType, node.Name, xml_node_father);
            }

            if (node.Attributes != null)
            {
                if (node.Attributes.Count > 0)
                {
                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        xml_father.add_attr_item(attr.Name, attr.Value);
                    }
                }
            }

            if (node.ChildNodes.Count > 0)
            {
                foreach (XmlNode node_child in node.ChildNodes)
                {
                    parse_note_child(node_child, xml_father);
                }
            }
        }
    }

    public void done_inp_editor_code()
    {
        this.ParseXML(this.inp_editor_text.text);
    }

    public void save_project()
    {
        app.carrot.ads.show_ads_Interstitial();
        if(this.xml_root!=null) 
            this.app.xml_manager.save_project(this.index_edit, this.xml_root.get_code_short());
        else
            this.app.xml_manager.save_project(this.index_edit, "<root></root>");
    }

    public void btn_export_file_xml()
    {
        this.app.carrot.play_sound_click();
        Carrot_File_Query q = new();
        q.Add_filter("XML data", "xml", "rdf", "rss");
        q.Add_filter("XHTML data", "xhtml", "xsl", "atom");
        q.SetDefaultFilter("xml");
        this.app.file.Save_file(q,Export_file_xml_done, Export_file_xml_cancel);
    }

    private void Export_file_xml_done(string[] s_path)
    {
        this.app.carrot.play_vibrate();
        FileBrowserHelpers.WriteTextToFile(s_path[0], this.xml_root.get_code_short());
        this.box_input = this.app.carrot.show_input(app.carrot.L("export","Xml Export"), "Exported xml file successfully at path ", s_path[0]);
        this.box_input.set_icon(this.sp_icon_export_file_xml);
        this.box_input.set_act_done(Act_close_msg_export);
    }

    private void Export_file_xml_cancel()
    {
        this.app.carrot.play_vibrate();
        this.app.carrot.play_sound_click();
    }

    private void Act_close_msg_export(string s_data)
    {
        if (this.box_input != null) this.box_input.close();
    }

    public void Btn_public_project()
    {
        string s_id_user_login = this.app.carrot.user.get_id_user_login();
        if (s_id_user_login != "")
        {
            app.carrot.Show_msg(app.carrot.L("upload_project", "Upload Project"), app.carrot.L("upload_project_question", "Are you sure you want to publish and back up this project online?"), () =>
            {
                app.carrot.show_loading();
                string s_data_code = PlayerPrefs.GetString("xml_" +this.index_edit+"_data");
                IDictionary data = (IDictionary)Json.Deserialize("{}");
                data["id"] = "code" + this.app.carrot.generateID();
                data["title"] = PlayerPrefs.GetString("xml_"+this.index_edit+"_name","");
                data["describe"] = PlayerPrefs.GetString("xml_" + this.index_edit + "_describe","");
                data["code_theme"] = "docco.min.css";
                data["code_type"] = "xml";
                data["code"] = ConvertXMLToHTMLEntities(s_data_code);
                data["user_id"] = app.carrot.user.get_id_user_login();
                data["user_lang"] = app.carrot.user.get_lang_user_login();
                data["status"] = "pending";

                string s_data_json = app.carrot.server.Convert_IDictionary_to_json(data);
                app.carrot.server.Add_Document_To_Collection("code", data["id"].ToString(), s_data_json, Act_upload_project_done, app.Act_server_fail);
            });
        }
        else
        {
            app.carrot.user.show_login(this.Btn_public_project);
        }
    }

    private string ConvertXMLToHTMLEntities(string xmlContent)
    {
        string htmlEntitiesContent = xmlContent;
        htmlEntitiesContent = Regex.Replace(htmlEntitiesContent, "&", "&amp;");
        htmlEntitiesContent = Regex.Replace(htmlEntitiesContent, "<", "&lt;");
        htmlEntitiesContent = Regex.Replace(htmlEntitiesContent, ">", "&gt;");
        htmlEntitiesContent = Regex.Replace(htmlEntitiesContent, "\"", "&quot;");
        htmlEntitiesContent = Regex.Replace(htmlEntitiesContent, "'", "&apos;");
        return htmlEntitiesContent;
    }

    public string ConvertHTMLEntitiesToXML(string xmlContent)
    {
        string htmlEntitiesContent = xmlContent;
        htmlEntitiesContent = Regex.Replace(htmlEntitiesContent, "&amp;", "&");
        htmlEntitiesContent = Regex.Replace(htmlEntitiesContent, "&lt;", "<");
        htmlEntitiesContent = Regex.Replace(htmlEntitiesContent, "&gt;", ">");
        htmlEntitiesContent = Regex.Replace(htmlEntitiesContent, "&quot;", "\"");
        htmlEntitiesContent = Regex.Replace(htmlEntitiesContent, "&apos;", "'");
        return htmlEntitiesContent;
    }

    private void Act_upload_project_done(string s_data)
    {
        app.carrot.hide_loading();
        app.carrot.Show_msg(app.carrot.L("upload_project", "Upload Project"), app.carrot.L("upload_project_success", "Successfully backed up the project online, you can share and restore the project through your account"), Msg_Icon.Success);
    }

    public void Btn_show_edit_project()
    {
        app.carrot.play_sound_click();
        this.box = this.app.carrot.Create_Box();
        this.box.set_icon(this.app.carrot.user.icon_user_edit);
        this.box.set_title(app.carrot.L("edit_info", "Edit Info project"));

        this.item_edit_name = this.box.create_item("item_name");
        this.item_edit_name.set_icon(this.app.carrot.icon_carrot_database);
        this.item_edit_name.set_title(app.carrot.L("project_name","Project Name"));
        this.item_edit_name.set_tip(app.carrot.L("project_name_tip", "Give your project a memorable name"));
        this.item_edit_name.set_type(Box_Item_Type.box_value_input);
        this.item_edit_name.check_type();
        this.item_edit_name.set_val(this.s_name_project);

        this.item_edit_describe= this.box.create_item("item_describe");
        this.item_edit_describe.set_icon(app.carrot.user.icon_user_info);
        this.item_edit_describe.set_title(app.carrot.L("project_describe", "Project Describe"));
        this.item_edit_describe.set_tip(app.carrot.L("project_describe_tip", "Write a description of this project"));
        this.item_edit_describe.set_type(Box_Item_Type.box_value_input);
        this.item_edit_describe.check_type();
        if (PlayerPrefs.GetString("xml_" + this.index_edit + "_describe", "") != "") this.item_edit_describe.set_val(PlayerPrefs.GetString("xml_" + this.index_edit + "_describe", ""));

        Carrot_Box_Btn_Panel panel_btn = this.box.create_panel_btn();
        Carrot_Button_Item btn_done=panel_btn.create_btn("btn_done");
        btn_done.set_icon_white(app.carrot.icon_carrot_done);
        btn_done.set_label(app.carrot.L("done", "Done"));
        btn_done.set_label_color(Color.white);
        btn_done.set_bk_color(app.carrot.color_highlight);
        btn_done.set_act_click(() => Act_edit_project_done());

        Carrot_Button_Item btn_cancel = panel_btn.create_btn("btn_cancel");
        btn_cancel.set_icon_white(app.carrot.icon_carrot_cancel);
        btn_cancel.set_label(app.carrot.L("cancel", "Cancel"));
        btn_cancel.set_label_color(Color.white);
        btn_cancel.set_bk_color(app.carrot.color_highlight);
        btn_cancel.set_act_click(() => Act_edit_project_cancel());
    }

    private void Act_edit_project_done()
    {
        if (this.item_edit_name.get_val().Trim() == "")
        {
            this.app.carrot.Show_msg(app.carrot.L("edit_info", "Edit Info project"), app.carrot.L("project_name_error", "Project name cannot be empty!"), Carrot.Msg_Icon.Error);
            app.carrot.play_sound_click();
            return;
        }

        app.carrot.play_sound_click();
        this.s_name_project=this.item_edit_name.get_val();
        PlayerPrefs.SetString("xml_" + this.index_edit + "_name",this.item_edit_name.get_val());
        PlayerPrefs.SetString("xml_" + this.index_edit + "_describe", this.item_edit_describe.get_val());
        this.txt_name_project.text = this.item_edit_name.get_val();
        box?.close();
    }

    private void Act_edit_project_cancel()
    {
        app.carrot.play_sound_click();
        box?.close();
    }

    public void show_menu_node(Xml_Item item_xml)
    {
        app.carrot.play_sound_click();
        this.box = app.carrot.Create_Box("box_menu");
        this.box.set_title(app.carrot.L("n_menu","Menu")+" - (" + item_xml.get_s_nodes() + ")");

        Carrot.Carrot_Box_Item item_edit = this.box.create_item("i_edit");
        item_edit.set_icon(item_xml.sp_icon_node_edit);
        item_edit.set_title(app.carrot.L("edit_info","Edit"));
        item_edit.set_tip(app.carrot.L("n_edit_tip","Edit node data"));
        item_edit.set_act(()=>{
            box?.close();
            app.xml.box_add.show_edit(item_xml);
        });

        if (item_xml.type != XmlNodeType.Text)
        {
            Carrot.Carrot_Box_Item item_attr = box.create_item("i_attr");
            item_attr.set_icon(item_xml.sp_icon_node_attr);
            item_attr.set_title(app.carrot.L("add_node_attr","Add attributes"));
            item_attr.set_tip(app.carrot.L("n_add_attr_tip", "Add properties for node"));
            item_attr.set_act(()=> {
                item_xml.btn_add_attr();
                box?.close();
            });

            List<Item_Attr> list_attr = item_xml.get_list_item_attr();
            if (list_attr.Count > 0)
            {
                Carrot.Carrot_Box_Item item_list_attr = box.create_item("i_list_attr");
                item_list_attr.set_icon(item_xml.sp_icon_node_list_attr);
                item_list_attr.set_title(app.carrot.L("n_list_attr", "List attributes"));
                item_list_attr.set_tip(app.carrot.L("n_list_attr_tip","Displays a list of node's attributes"));
                item_list_attr.set_act(()=> btn_show_list_attr(item_xml));
            }
        }

        Carrot.Carrot_Box_Item item_del = box.create_item("i_delete");
        item_del.set_icon(item_xml.sp_icon_node_del);
        item_del.set_title(app.carrot.L("delete","Delete"));
        item_del.set_tip(app.carrot.L("n_delete_tip","Delete the data node and its child nodes"));
        item_del.set_act(()=> Delete_node(item_xml));
    }

    public void Delete_node(Xml_Item xml_item)
    {
        app.carrot.play_sound_click();
        app.carrot.Show_msg(app.carrot.L("delete", "Delete"), app.carrot.L("n_delete_question", "Are you sure you want to remove this item?"), () =>
        {
            xml_item.Delete_all_child();
            Destroy(xml_item.gameObject);
            Destroy(xml_item);
            this.box?.close();
        });
    }

    public void btn_show_list_attr(Xml_Item xml_item)
    {
        this.box_sub = app.carrot.Create_Box("box_attr");
        this.box_sub.set_icon(xml_item.sp_icon_node_list_attr);
        this.box_sub.set_title(app.carrot.L("n_list_attr", "List attributes"));

        List<Item_Attr> list_attr = xml_item.get_list_item_attr();
        for (int i = 0; i < list_attr.Count; i++)
        {
            var index_item = i;
            var xml_item_data = xml_item;
            Carrot_Box_Item item_attr = this.box_sub.create_item("item_attr_" + i);
            item_attr.set_icon(xml_item.sp_icon_node_attr);
            item_attr.set_title(list_attr[i].get_s_name());
            item_attr.set_tip(list_attr[i].get_s_value());

            var attr_set = list_attr[i];
            item_attr.set_act(() => show_edit_attr(attr_set));

            Carrot_Box_Btn_Item btn_del = item_attr.create_item();
            btn_del.set_color(Color.red);
            btn_del.set_icon(app.carrot.sp_icon_del_data);
            btn_del.set_act(() => Delete_attr_node(index_item, xml_item_data, item_attr.gameObject));
        }
    }

    private void Delete_attr_node(int index,Xml_Item xml_item,GameObject item_obj)
    {
        xml_item.Delete_attr(index);
        Destroy(item_obj);
        this.box_sub?.close();
    }

    private void show_edit_attr(Item_Attr attr)
    {
        box_sub?.close();
        box?.close();
        app.xml.box_add.show_edit_attr(attr);
    }

    public void Btn_save()
    {
        app.carrot.ads.show_ads_Interstitial();
        app.carrot.play_sound_click();
        if(this.xml_root!=null)
            PlayerPrefs.SetString("xml_" + this.index_edit + "_data", this.xml_root.get_code_short());
        else
            PlayerPrefs.SetString("xml_" + this.index_edit + "_data", "<root></root>");
        app.carrot.Show_msg(app.carrot.L("editor", "Data design"),app.carrot.L("p_update_success","Successful project update!"), Msg_Icon.Success);
    }
}
