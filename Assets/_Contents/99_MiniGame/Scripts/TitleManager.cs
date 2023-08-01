using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Button _startButton;

    void Start()
    {
        
    }

    
    public async void ClickStartButtonAsync()
    {
        await _startButton.OnClickAsync();
    }
}
