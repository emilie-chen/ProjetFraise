using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEditor.SearchService.Scene;

public class reload : MonoBehaviour
{
    // Start is called before the first frame update
    public void reloadSc()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
