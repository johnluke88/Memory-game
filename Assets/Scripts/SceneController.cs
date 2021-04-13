using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public int gridRows = 0;
    public int gridCols = 0;
    public const float offsetX = 1.5f;
    public const float offsetY = 2.5f;

    [SerializeField] private MemoryCard originalCard;
    [SerializeField] private Sprite[] images;

    private MemoryCard _firstRevealed;
    private MemoryCard _secondRevealed;
    private MemoryCard _thirdRevealed;

    private int _score = 0;

    [SerializeField] private TextMesh scoreLabel;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 startPos = originalCard.transform.position;
        int[] numbers = new int[images.Length*3];
        for (int i = 0; i < numbers.Length; i++)
        {
            numbers[i] = (int)(i / 3);
        }

        gridCols = numbers.Length / 2;
        int carry = numbers.Length % 2;
        gridRows = numbers.Length / gridCols + carry; 

        numbers = ShuffleArray(numbers);

        for (int i = 0; i < gridCols; i++) {
            for (int j = 0; j < gridRows; j++) {
                MemoryCard card;
                if (i == 0 && j == 0)
                {
                    card = originalCard;
                }
                else 
                {
                    card = Instantiate(originalCard) as MemoryCard;
                }

                int index =  j * gridCols + i;
                int id = numbers[index];
                card.SetCard(id, images[id]);

                float posX = (offsetX * i) + startPos.x;
                float posY =  -(offsetY * j) + startPos.y;
                card.transform.position = new Vector3(posX, posY, startPos.z);
            }
        }
    }

    private int[] ShuffleArray(int[] numbers)
    {
        int[] newArray = numbers.Clone() as int[];
        for (int i = 0; i < newArray.Length; i++) {
            int tmp = newArray[i];
            int r = Random.Range(i, newArray.Length);
            newArray[i] = newArray[r];
            newArray[r] = tmp;
        }
        return newArray;
    }
    public bool canReveal
    {
        get { return _secondRevealed == null || _thirdRevealed == null; }
    }

    public void CardRevealed(MemoryCard card) {
        if (_firstRevealed == null)
        {
            _firstRevealed = card;
            Debug.Log("_firstRevealed ID: " + _firstRevealed.id);
        }
        else if (_secondRevealed == null)
        {
            _secondRevealed = card;
            Debug.Log("_secondRevealed ID: " + _secondRevealed.id);
        }
        else
        {
            _thirdRevealed = card;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        if (_firstRevealed.id == _thirdRevealed.id && _secondRevealed.id == _thirdRevealed.id)
        {
            _score++;
            scoreLabel.text = "Score: " + _score;
        }
        else 
        {
            yield return new WaitForSeconds(.5f);

            _firstRevealed.Unreveal();
            _secondRevealed.Unreveal();
            _thirdRevealed.Unreveal();
        }

        _firstRevealed = null;
        _secondRevealed = null;
        _thirdRevealed = null;
    }

    public void Restart() {
        SceneManager.LoadScene("SampleScene");
    }
}
