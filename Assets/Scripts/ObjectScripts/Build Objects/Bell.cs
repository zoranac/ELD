using UnityEngine;
using System.Collections;


public class Bell : PassiveConnectorObject {
    public AudioClip[] bells = new AudioClip[10];
    bool AudioPlayed = false;
	// Update is called once per frame
	void Update () {
        if (dotTile.GetComponent<DotTileScript>().Power > 0 && dotTile.GetComponent<DotTileScript>().Power <= 10)
        {
            if (!AudioPlayed)
            {
                GetComponent<AudioSource>().clip = bells[dotTile.GetComponent<DotTileScript>().Power - 1];
                GetComponent<AudioSource>().Play();
                AudioPlayed = true;
            }
        }
        else if (dotTile.GetComponent<DotTileScript>().Power <= 0)
        {
            AudioPlayed = false;
        }
	}
}
