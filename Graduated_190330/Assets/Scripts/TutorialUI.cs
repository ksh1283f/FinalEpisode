using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : uiSingletone<TutorialUI>
{
    public Dialogue dialogue;
    protected override void Awake()
    {
        uiType = E_UIType.Tutorial;
        base.Awake();
    }

    private void Start()
    {
        Close();
    }

    public override void Show()
    {
        // 특수한 조건에서만
        base.Show();
    }

    public override void Close()
    {
        base.Close();

    }
}
