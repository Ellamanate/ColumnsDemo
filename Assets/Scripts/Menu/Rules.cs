using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class Rules : MonoBehaviour
{
    [SerializeField] private Text description;
    [SerializeField] private Animator animator;
    [SerializeField] private ButtonExtension nextButton;
    [SerializeField] private ButtonExtension previousButton;
    [SerializeField] private ButtonExtension exitButton;
    [SerializeField] private List<string> descriptionText;
    private int number = 0;

    public void NextRule()
    {
        number++;
        animator.SetBool("Rule_" + number.ToString(), true);
        description.text = descriptionText[number - 1];

        if (number > 1) 
        {
            if (number == 4)
            {
                nextButton.gameObject.SetActive(false);
                exitButton.gameObject.SetActive(true);
            }

            previousButton.gameObject.SetActive(true);
            animator.SetBool("Rule_" + (number - 1).ToString(), false);
        }
        else if (number == 1)
            previousButton.gameObject.SetActive(false);
    }

    public void PreviousRule()
    {
        animator.SetBool("Rule_" + number.ToString(), false);
        number--;

        if (number < 4)
        {
            nextButton.gameObject.SetActive(true);
            exitButton.gameObject.SetActive(false);
        }

        if (number > 0)
        {
            if (number == 1)
                previousButton.gameObject.SetActive(false);

            animator.SetBool("Rule_" + number.ToString(), true);
            description.text = descriptionText[number - 1];
        }
    }

    public void DropRule()
    {
        nextButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(false);
        animator.SetBool("Rule_1", false);
        animator.SetBool("Rule_2", false);
        animator.SetBool("Rule_3", false);
        animator.SetBool("Rule_4", false);
        number = 0;
    }
}