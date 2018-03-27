using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Minecraft
{
    public class Items : MonoBehaviour



    {
        public Image[] package;
        public Text[] package_num;
        public class Inventtory
        {
            public int id = 0;
            public int Slot;
            public int Count;
            public int Damage;
            public Inventtory() { }
            public Inventtory(int _id,int _slot,int _count,int _damage)
            {
                this.id = _id;
                this.Slot = _slot;
                this.Count = _count;
                this.Damage = _damage;
            }
        }
        public class InventtoryList
        {
            public List<Inventtory> _myItems = new List<Inventtory>();
            public InventtoryList()
            {
                Inventtory it = new Inventtory();
                this._myItems.Add(it);
            }
        }
        // Use this for initialization
        void Start()
        {
            package = this.GetComponentsInChildren<Image>();
            package_num = this.GetComponentsInChildren<Text>();
            InventtoryList myItems = readData();
            for (int i = 0; i < myItems._myItems.Count; ++i)
            {
                package[myItems._myItems[i].Slot + 1].enabled = true;
                package_num[myItems._myItems[i].Slot].text = myItems._myItems[i].Count.ToString();
                package[myItems._myItems[i].Slot + 1].overrideSprite = Resources.Load("Texture/"+ItemDatabase.GetItemById(myItems._myItems[i].id).name, typeof(Sprite)) as Sprite;
            }
        }

        // Update is called once per frame
        /*
        void Update()
        {
            for (int i = 0; i < 9; i++)
            {
                if (PlayerPrefs.GetInt("??") > 0)
                {
                    package[i].overrideSprite = Resources.Load("Texture/Dirt", typeof(Sprite)) as Sprite;
                    package[i].enabled = true;
                    package_num[i].text = PlayerPrefs.GetInt("dirt").ToString();
                    package_num[i].enabled = true;
                }
                else
                {
                    package[i].enabled = false;
                    package_num[i].enabled = false;
                }
            }
        }
        */
        public static void refresh(InventtoryList myItems)
        {
            Image[] package_;
            Text[] package_num_;
            package_ = GameObject.Find("Package").GetComponentsInChildren<Image>();
            package_num_ = GameObject.Find("Package").GetComponentsInChildren<Text>();
            for (int i = 0; i < myItems._myItems.Count; ++i)
            {
                    if (myItems._myItems[i].Count < 1)
                    {
                        package_[myItems._myItems[i].Slot + 1].enabled = false;
                        package_num_[myItems._myItems[i].Slot].text = "";
                    }
                    else
                    {
                        package_[myItems._myItems[i].Slot + 1].enabled = true;
                        package_num_[myItems._myItems[i].Slot].text = myItems._myItems[i].Count.ToString();
                        package_[myItems._myItems[i].Slot + 1].overrideSprite = Resources.Load("Texture/" + ItemDatabase.GetItemById(myItems._myItems[i].id).name, typeof(Sprite)) as Sprite;
                    }
            }
        }
        //保存人物背包信息
        public static void SavePackageData(Inventtory data)
        {
            InventtoryList _myItems=null;
            string filePath = Application.dataPath + @"/Resources/Settings/package.json";
            if (!File.Exists(filePath))
            {
                _myItems = new InventtoryList();
                _myItems._myItems.Add(data);
            }
            else
            {
                bool bFind = false;
                _myItems = PlayerIO.getPackItems();
                for (int i = 0; i < _myItems._myItems.Count; ++i)
                {
                    Inventtory saveData = _myItems._myItems[i];
                    if (data.Slot == saveData.Slot)
                    {
                        saveData.id = data.id;
                        saveData.Count = data.Count;
                        saveData.Damage = data.Damage;
                        bFind = true;
                        break;
                    }
                }
                if (!bFind)
                    _myItems._myItems.Add(data);
            }

            FileInfo file = new FileInfo(filePath);
            StreamWriter sw = file.CreateText();
            string json = JsonMapper.ToJson(_myItems);
            sw.WriteLine(json);
            sw.Close();
            sw.Dispose();
        }
        
        public static void save(InventtoryList data)
        {
            for(int i = 0; i < data._myItems.Count; i++)
            {
                SavePackageData(data._myItems[i]);
            }
        }
        //读取人物物品信息
        public static InventtoryList readData()
        {
            string json1;
            
            string filePath = Application.dataPath + @"/Resources/Settings/package.json";
            if (!File.Exists(filePath))
            {
                return null;
            }
            else
            {
                StreamReader sr = new StreamReader(Application.dataPath + @"/Resources/Settings/package.json");
                if (sr == null)
                {
                    sr.Close();
                    return null;
                }
                json1 = sr.ReadToEnd();
                if (json1.Length > 0)
                {
                    InventtoryList Il = JsonMapper.ToObject<InventtoryList>(json1);
                    sr.Close();
                    return Il;
                }
            }
            return null;
        }
        
        
    }
}
