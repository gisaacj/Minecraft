using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Minecraft
{
    public class PlayerIO : MonoBehaviour
    {

        public static PlayerIO currentPlayerIO;
        public float maxInteractDistance = 8;
        private int selectedSlot = 0;
        public bool resetCamera = false;
        public Vector3 campos;
        public Animator playerAnimator;
        private bool choosing = false;
        private GameObject menu;
        private Items.InventtoryList itms;
        private bool full = false;
        private byte selectedInventtory;

        // Use this for initialization
        void Start()
        {
            currentPlayerIO = this;
            menu = GameObject.FindGameObjectWithTag("Menu");
            menu.SetActive(false);
            itms = new Items.InventtoryList();
            itms = Items.readData();
            /*
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 144;
            */
        }
        public static Items.InventtoryList getPackItems()
        {
            return PlayerIO.currentPlayerIO.itms;
        }
        // Update is called once per frame
        void Update()
        {
            if (choosing == false)
                playerAnimator.SetBool("walking", Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D));
            if (GameObject.FindWithTag("FPSController").transform.position.y < -30)
            {
                Debug.Log("die!");
                GameObject.FindWithTag("FPSController").transform.position = new Vector3(GameObject.FindWithTag("FPSController").transform.position.x, 60, GameObject.FindWithTag("FPSController").transform.position.z);
            }
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
                RaycastHit hit;
                float rayDistance = maxInteractDistance;
                if (!resetCamera)
                {
                    rayDistance *= 3.14159f;
                }
                if (Physics.Raycast(ray, out hit, rayDistance))
                {
                    Chunk chunk = hit.transform.GetComponent<Chunk>();
                    if (chunk == null)
                    {
                        return;
                    }
                    if (choosing == false)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            Vector3 p = hit.point;
                            p -= hit.normal / 4;
                            byte block_byte = chunk.GetByte(p);
                            if (chunk.SetBrick(0, p) == true)
                            {
                                GameObject block = (GameObject)Resources.Load("Prefabs/" + ItemDatabase.GetItemById(block_byte).name);
                                ItemDrop(block, p, ItemDatabase.GetItemById(block_byte).name);
                            }
                        }
                        if (Input.GetMouseButtonDown(1))
                        {
                            Vector3 p = hit.point;
                            if (selectedSlot != -1)
                            {
                                p += hit.normal / 4;
                                if (Mathf.Floor(transform.position.x) == Mathf.Floor(p.x) && Mathf.Floor(transform.position.y) - 1 == Mathf.Floor(p.y) && Mathf.Floor(transform.position.z) == Mathf.Floor(p.z))
                                    return;
                                if (Mathf.Floor(transform.position.x) == Mathf.Floor(p.x) && Mathf.Floor(transform.position.y) == Mathf.Floor(p.y) && Mathf.Floor(transform.position.z) == Mathf.Floor(p.z))
                                    return;
                                for (int i = 0; i < itms._myItems.Count; ++i)
                                {
                                    if (selectedSlot == itms._myItems[i].Slot)
                                    {
                                        selectedInventtory = (byte)itms._myItems[i].id;
                                        itms._myItems[i].Count--;
                                        chunk.SetBrick(selectedInventtory, p);
                                        Items.refresh(itms);
                                        if (itms._myItems[i].Count < 1)
                                        {
                                            itms._myItems.Remove(itms._myItems[i]);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (menu.GetComponent<Canvas>().enabled)
                {
                    Time.timeScale = 1;
                    menu.SetActive(false);
                    menu.GetComponent<Canvas>().enabled = false;
                    this.gameObject.GetComponent<MouseLook>().enabled = true;
                    this.gameObject.transform.parent.GetComponentInParent<MouseLook>().enabled = true;
                    Cursor.visible = false;
                    choosing = false;
                }
                else
                {
                    Time.timeScale = 0;
                    menu.SetActive(true);
                    menu.GetComponent<Canvas>().enabled = true;
                    this.gameObject.GetComponent<MouseLook>().enabled = false;
                    this.gameObject.transform.parent.GetComponentInParent<MouseLook>().enabled = false;
                    Cursor.visible = true;
                    choosing = true;
                }
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                if (!resetCamera)
                {
                    Camera.main.transform.localPosition -= Vector3.forward * 3.14159f;
                }
                else
                {
                    Camera.main.transform.position = transform.position;
                }
                resetCamera = !resetCamera;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                selectedSlot = 0;
                selectitem(selectedSlot);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                selectedSlot = 1;
                selectitem(selectedSlot);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                selectedSlot = 2;
                selectitem(selectedSlot);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                selectedSlot = 3;
                selectitem(selectedSlot);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                selectedSlot = 4;
                selectitem(selectedSlot);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                selectedSlot = 5;
                selectitem(selectedSlot);
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                selectedSlot = 6;
                selectitem(selectedSlot);
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                selectedSlot = 7;
                selectitem(selectedSlot);
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                selectedSlot = 8;
                selectitem(selectedSlot);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                
            }
            ItemCollect();
        }
        void ItemDrop(GameObject d, Vector3 v, string name_)
        {
            if (d.GetComponent<Rigidbody>() == false)
            {
                d.AddComponent<Rigidbody>();
            }
            d.name = name_;
            Instantiate(d, v, Quaternion.identity);
        }
        void ItemCollect()
        {
            Items.Inventtory item_ = new Items.Inventtory();
            int flag;
            float dis = 2.4f;
            Collider[] collider = Physics.OverlapSphere(transform.position, dis);
            foreach (Collider col in collider)
            {
                if (col.tag == "Drop_item")
                {
                    GameObject player = GameObject.FindGameObjectWithTag("FPSController");
                    if (full == false)
                    {
                        Physics.IgnoreCollision(col, player.GetComponent<Collider>());
                        col.transform.position = Vector3.MoveTowards(col.transform.position, transform.position, 8f * Time.deltaTime);
                        if (Vector3.Distance(col.transform.position, transform.position) < 0.4f)
                        {
                            Destroy(col.gameObject);
                            bool bFind = false;
                            int next_Slot = -1;
                            for (int i = 0; i < itms._myItems.Count; ++i)
                            {
                                if (ItemDatabase.GetItemByName(col.gameObject.name.Remove(col.gameObject.name.Length - 7, 7)).id == itms._myItems[i].id)
                                {
                                    itms._myItems[i].Count++;
                                    bFind = true;
                                    break;
                                }
                            }
                            if (bFind == false)
                            {
                                for (int j = 0; j < 9; j++)
                                {
                                    flag = 0;
                                    for (int i = 0; i < itms._myItems.Count; i++)
                                    {
                                        if (j != itms._myItems[i].Slot)
                                            flag++;
                                    }
                                    if (flag == itms._myItems.Count)
                                    {
                                        next_Slot = j;
                                        item_.Count = 1;
                                        item_.Slot = next_Slot;
                                        item_.id = ItemDatabase.GetItemByName(col.gameObject.name.Remove(col.gameObject.name.Length - 7, 7)).id;
                                        item_.Damage = 0;
                                        itms._myItems.Add(item_);
                                        break;
                                    }
                                }
                                if (next_Slot == -1)
                                {
                                    full = true;
                                }
                            }
                            Items.refresh(itms);
                        }
                    }
                }
            }
        }
        void selectitem(int selectslot)
        {
            switch (selectslot)
            {
                case 0: GameObject.Find("SelectedSlot").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-210, -271); break;
                case 1: GameObject.Find("SelectedSlot").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-158, -271); break;
                case 2: GameObject.Find("SelectedSlot").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-105, -271); break;
                case 3: GameObject.Find("SelectedSlot").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-53, -271); break;
                case 4: GameObject.Find("SelectedSlot").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -271); break;
                case 5: GameObject.Find("SelectedSlot").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(53, -271); break;
                case 6: GameObject.Find("SelectedSlot").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(105, -271); break;
                case 7: GameObject.Find("SelectedSlot").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(158, -271); break;
                case 8: GameObject.Find("SelectedSlot").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(211, -271); break;
            }
        }
    }
}