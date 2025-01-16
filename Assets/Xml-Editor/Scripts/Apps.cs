using Carrot;
using UnityEngine;
using UnityEngine.UI;

public class Apps : MonoBehaviour
{
    public Carrot.Carrot carrot;
    public Carrot.Carrot_File file;
    public Xml_Editor xml;
    public Xml_Manager xml_manager;
    public IronSourceAds ads;

    [Header("Ui")]
    public InputField inp_xml_name;
    public AudioSource sound_background;
    private int scores_rank = 0;
    private string link_deep_app = "";

    void Start()
    {
        this.link_deep_app = Application.absoluteURL;
        Application.deepLinkActivated += onDeepLinkActivated;

        this.carrot.Load_Carrot(this.check_exit_app);
        this.ads.On_Load();
        this.carrot.act_after_delete_all_data = this.act_delete_all_data;
        this.carrot.game.load_bk_music(this.sound_background);
        this.carrot.game.act_click_watch_ads_in_music_bk=this.ads.ShowRewardedVideo;
        this.ads.onRewardedSuccess=this.carrot.game.OnRewardedSuccess;
        this.carrot.act_buy_ads_success=this.ads.RemoveAds;

        if (carrot.os_app == OS.Android)
            file.type = Carrot_File_Type.SimpleFileBrowser;
        else 
            file.type = Carrot_File_Type.StandaloneFileBrowser;

        this.xml.on_load();
        this.xml_manager.on_load();

        this.scores_rank = PlayerPrefs.GetInt("scores_rank", 0);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) this.carrot.delay_function(3f, this.check_link_deep_app);
    }

    private void onDeepLinkActivated(string url)
    {
        this.link_deep_app = url;
        if (this.carrot != null) this.carrot.delay_function(1f, this.check_link_deep_app);
    }

    public void check_link_deep_app()
    {
        if (this.link_deep_app.Trim() != "")
        {
            if (this.carrot.is_online())
            {
                if (this.link_deep_app.Contains("codexml:"))
                {
                    string id_project = this.link_deep_app.Replace("codexml://show/", "");
                    Debug.Log("Get Project id:" + id_project);
                    xml_manager.Get_project_by_id(id_project);
                    this.link_deep_app = "";
                }
            }
        }
    }

    [ContextMenu("Test Deep Link")]
    public void test_deep_link()
    {
        this.onDeepLinkActivated("codexml://show/code033fd7e38e374ed89872e2c81b845c34");
    }


    private void check_exit_app()
    {
        if (this.xml.box_add.pane_add.activeInHierarchy)
        {
            this.xml.box_add.btn_close();
            this.carrot.set_no_check_exit_app();
        }else if (this.xml.panel_editor.activeInHierarchy)
        {
            this.xml.closer_project();
            this.carrot.set_no_check_exit_app();
        }
    }

    public void btn_setting()
    {
        this.carrot.Create_Setting();
    }

    public void btn_rate()
    {
        this.carrot.show_rate();
    }

    public void btn_user()
    {
        this.carrot.user.show_login();
    }

    public void btn_ranks()
    {
        this.carrot.game.Show_List_Top_player();
    }

    public void btn_create_project()
    {
        this.ads.show_ads_Interstitial();
        this.carrot.play_sound_click();
        string s_file_name = this.inp_xml_name.text;
        if (s_file_name.Trim() != "")
        {
            this.ads.HideBannerAd();
            this.xml.create_project(this.inp_xml_name.text);
            this.add_scores_rank();
        } 
        else
        {
            this.carrot.play_vibrate();
            this.carrot.Show_msg(carrot.L("create_project", "Create Project"), carrot.L("project_name_error","Project name cannot be empty!"), Carrot.Msg_Icon.Error);
        }
    }

    public void btn_show_create_project()
    {
        this.ads.show_ads_Interstitial();
        this.carrot.play_sound_click();
        this.xml_manager.panel_new_project.SetActive(true);
    }

    public void btn_close_create_project()
    {
        this.carrot.play_sound_click();
        this.xml_manager.panel_new_project.SetActive(false);
    }

    private void act_delete_all_data()
    {
        this.carrot.delay_function(1f, this.Start);
    }

    public void btn_import_xml()
    {
        this.carrot.play_sound_click();
        this.xml.Show_import();
    }

    private void add_scores_rank()
    {
        this.scores_rank++;
        PlayerPrefs.SetInt("scores_rank", this.scores_rank);

        if (Random.Range(0, 2) == 1)
        {
            this.carrot.game.update_scores_player(this.scores_rank);
        }
    }

    public void Act_server_fail(string s_error)
    {
        carrot.hide_loading();
        carrot.Show_msg("Error", s_error, Msg_Icon.Error);
        carrot.play_vibrate();
    }
}
