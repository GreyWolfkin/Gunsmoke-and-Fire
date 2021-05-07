using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {

    [SerializeField] AudioSource[] sfxFiles;
    [SerializeField] string[] stateNames;

    public AudioSource[] getSFXFiles() {
        return sfxFiles;
    }

    public string[] getStateNames() {
        return stateNames;
    }

}
