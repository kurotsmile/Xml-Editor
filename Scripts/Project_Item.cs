using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Project_Item : MonoBehaviour
{
    public Text txt_name_project;
    public int index;

    public void click()
    {
        GameObject.Find("App").GetComponent<Apps>().xml.open_project_xml_by_index(this.index);
    }
}
