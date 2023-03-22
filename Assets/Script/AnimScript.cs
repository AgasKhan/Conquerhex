using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimScript : MonoBehaviour
{

    public Menu menu;
   
    public void AdiosMundoCruel()
    {

        menu.ShowMenu("Derrota!", "Has sido derrotado", false, true);
        

        Time.timeScale = 0;

        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AttackPlayer()
    {
        AudioManager.instance.Play("parryStart");
    }

    public void WalkPlayerLeft()
    {
      
        AudioManager.instance.Play("izq"+Random.Range(1,3));
    }

    public void WalkPlayerRight()
    {
      
        AudioManager.instance.Play("der" + Random.Range(1, 3));
    }

    public void DestruccionLetal()
    {
        Destroy(transform.parent.gameObject,1);
    }

    

}
