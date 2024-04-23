using Carrot;
using System.Collections;
using UnityEngine;

public class Xml_Manager : MonoBehaviour
{

    [Header("Emp Manager Project")]
    public Apps apps;
    public GameObject item_project_prefab;

    [Header("Emp Ui")]
    public GameObject panel_new_project;
    public GameObject panel_all_project;
    public Transform tr_all_project;
    public GameObject obj_btn_create_project;
    public GameObject obj_btn_cancel_create_project;

    private Carrot_Box box = null;

    private int xml_length = 0;

    public void on_load()
    {
        this.xml_length = PlayerPrefs.GetInt("xml_length",0);
        this.check_show_contain();
    }

    private void check_show_contain()
    {
        this.panel_all_project.SetActive(false);
        this.panel_new_project.SetActive(false);

        if (this.xml_length== 0)
        {
            this.panel_new_project.SetActive(true);
            this.obj_btn_create_project.SetActive(false);
            this.obj_btn_cancel_create_project.SetActive(false);
        } 
        else
        {
            this.panel_all_project.SetActive(true);
            this.obj_btn_create_project.SetActive(true);
            this.obj_btn_cancel_create_project.SetActive(true);
            this.on_load_all_data();
        }     
    }

    public int add_project(string s_name,string s_data)
    {
        PlayerPrefs.SetString("xml_"+this.xml_length+"_name", s_name);
        PlayerPrefs.SetString("xml_"+this.xml_length+"_data", s_data);
        this.xml_length++;
        PlayerPrefs.SetInt("xml_length", this.xml_length);
        this.check_show_contain();
        return this.xml_length - 1;
    }

    public void save_project(int index,string s_data)
    {
        PlayerPrefs.SetString("xml_" + index + "_data", s_data);
    }

    public void on_load_all_data()
    {
        this.apps.carrot.clear_contain(this.tr_all_project);

        int count_true_box = 0;
        for(int i = 0; i < this.xml_length; i++)
        {
            string s_name_project = PlayerPrefs.GetString("xml_" + i + "_name");

            if (s_name_project != "")
            {
                GameObject obj_xml_project = Instantiate(this.item_project_prefab);
                obj_xml_project.transform.SetParent(this.tr_all_project);
                obj_xml_project.transform.localPosition = Vector3.zero;
                obj_xml_project.transform.localScale = new Vector3(1f, 1f, 1f);

                Project_Item p_item = obj_xml_project.GetComponent<Project_Item>();
                p_item.txt_name_project.text = PlayerPrefs.GetString("xml_" + i + "_name");
                p_item.index = i;
                count_true_box++;
            }
        }

        if (count_true_box == 0)
        {
            this.xml_length = 0;
            PlayerPrefs.DeleteKey("xml_length");
            this.check_show_contain();
        }
    }

    public void delete_project(int index_project)
    {
        PlayerPrefs.DeleteKey("xml_" + index_project + "_name");
        PlayerPrefs.DeleteKey("xml_" + index_project + "_data");
        this.on_load_all_data();
    }

    public int get_length_project()
    {
        return this.xml_length;
    }

    public void Show_list_online_project()
    {
        apps.carrot.show_loading();
        StructuredQuery q = new("code");
        q.Add_where("code_type", Query_OP.EQUAL, "xml");
        this.apps.carrot.server.Get_doc(q.ToJson(),get_all_data_project);
    }

    private void get_all_data_project(string s_data)
    {
        Debug.Log(s_data);
        apps.carrot.hide_loading();
        Fire_Collection fc = new(s_data);

        if (!fc.is_null)
        {
            this.box = this.apps.carrot.Create_Box();
            this.box.set_icon(this.apps.carrot.lang.icon);
            this.box.set_title("List Online Project");

            for (int i = 0; i < fc.fire_document.Length; i++)
            {
                IDictionary data_project = fc.fire_document[i].Get_IDictionary();
                Carrot_Box_Item item_project = this.box.create_item("item_project_" + i);
                item_project.set_icon(this.apps.carrot.icon_carrot_database);
                item_project.set_title(data_project["title"].ToString());
                if (data_project["describe"] != null) item_project.set_tip(data_project["describe"].ToString());
                else item_project.set_tip("Project online");

                Carrot_Box_Btn_Item btn_download = item_project.create_item();
                btn_download.set_icon(apps.carrot.icon_carrot_download);
                btn_download.set_color(apps.carrot.color_highlight);
                btn_download.set_act((null));
            }
        }
        else
        {
            apps.carrot.Show_msg("List Online Project", "None list",Msg_Icon.Alert);
        }
    }

    private void Act_download_project(IDictionary data)
    {

    }
}
