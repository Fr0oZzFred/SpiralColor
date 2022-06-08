using UnityEngine;
using UnityEngine.UI;

public class LevelRow : MonoBehaviour {

    [SerializeField] ScriptableLevelRow levelRow;

    [SerializeField] Image shape1, shape2, shape3, shape4, gems;

    [SerializeField] TMPro.TMP_Text gemsText;

    Button button;


    private void Start() {
        Init();
    }

    public void SetGemsProgression(string text) {
        gemsText.SetText(text);
    }

    void Init() {
        if (!levelRow) return;

        shape1.sprite = levelRow.shape1;
        shape2.sprite = levelRow.shape2;
        shape3.sprite = levelRow.shape3;
        shape4.sprite = levelRow.shape4;

        shape1.gameObject.SetActive(levelRow.shape1);
        shape2.gameObject.SetActive(levelRow.shape2);
        shape3.gameObject.SetActive(levelRow.shape3);
        shape4.gameObject.SetActive(levelRow.shape4);

        gems.sprite = levelRow.gems;

        button = GetComponent<Button>();
    }

    public void InvokeButtonEvents() {
        button.onClick.Invoke();
    }

    public void SetLevel(ScriptableLevelRow scriptableObject) {
        shape1.sprite = scriptableObject.shape1;
        shape2.sprite = scriptableObject.shape2;
        shape3.sprite = scriptableObject.shape3;
        shape4.sprite = scriptableObject.shape4;

        shape1.gameObject.SetActive(scriptableObject.shape1);
        shape2.gameObject.SetActive(scriptableObject.shape2);
        shape3.gameObject.SetActive(scriptableObject.shape3);
        shape4.gameObject.SetActive(scriptableObject.shape4);

        gems.sprite = scriptableObject.gems;
    }
}
