using UnityEngine;
using UnityEngine.UI;

public class Item_Attr : MonoBehaviour
{
    public Text txt_attr;

    private string s_name;
    private string s_val;

    public void on_load(string s_name,string s_val)
    {
        this.s_name = s_name;
        this.s_val = s_val;
        this.txt_attr.text = this.s_name + "=" + this.s_val;
    }

    public string get_s_name()
    {
        return this.s_name;
    }

    public string get_s_value()
    {
        return this.s_val;
    }
}
