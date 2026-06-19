using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Generator.Instance.CharacterSelectionSystem.SelectCharacter("player_mage");
        if (Input.GetKeyDown(KeyCode.Alpha2))
            Generator.Instance.CharacterSelectionSystem.SelectCharacter("player_wizard");

    }
}
