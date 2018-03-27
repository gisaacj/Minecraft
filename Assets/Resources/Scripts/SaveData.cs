using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Minecraft{
    public class SaveData : MonoBehaviour
    {

        public Button saveBtn;
        // Use this for initialization
        void Start()
        {
            saveBtn.onClick.AddListener(delegate () {
                Items.save(PlayerIO.getPackItems());
            });
        }

        // Update is called once per frame
        void Update()
        {
      
        }
    }
}
