using System.Collections.Generic;
using System.Linq;
using STG;

public class ChoiceSelector
{
    private int choiceLowerBound = 0;
    public int ChoiceLowerBound
    {
        get
        {
            return choiceLowerBound;
        }
    }
    private int choiceUpperBound = 1;
    public int ChoiceUpperBound
    {
        get
        {
            return choiceUpperBound;
        }
    }
    private int length = 0;
    private int currentChoice = 0;
    public int CurrentChoice
    {
        set
        {
            currentChoice = value;
        }
        get
        {
            return currentChoice;
        }
    }
    private bool isEveryChoiceDisabled = false;

    private HashSet<int> disabledChoices;

    public static ChoiceSelector CreateFrom0(int inclusiveUpperBound, HashSet<int> disabledList, int initialChoice)
    {
        var temp = new ChoiceSelector()
        {
            choiceLowerBound = 0,
            choiceUpperBound = inclusiveUpperBound,
            length = inclusiveUpperBound + 1,
            disabledChoices = disabledList,
            currentChoice = initialChoice,
        };
        temp.TrimDisabledChoices();
        return temp;
    }

    public void AddDisabledChoices(HashSet<int> toAdd)
    {
        disabledChoices.UnionWith(toAdd);
        TrimDisabledChoices();
    }

    public void RemoveDisabledChoices(HashSet<int> toRemove)
    {
        for (int i = 0; i < toRemove.Count; i++)
        {
            int choiceToRemove = toRemove.ElementAt(i);
            disabledChoices.Remove(choiceToRemove);
        }
        TrimDisabledChoices();
    }

    /// <summary>
    /// Trims list of disabled choices outside of lower/upper bounds.
    /// <br/>
    /// Also checks if all possible choices are disabled for future GetNext/GetPrevious.
    /// </summary>
    private void TrimDisabledChoices()
    {
        HashSet<int> trimmedDisabledChoices = new();
        bool isEveryChoiceDisabled = true;
        for (int i = choiceLowerBound; i <= choiceUpperBound; i++)
        {
            if (disabledChoices.Contains(i))
            {
                trimmedDisabledChoices.Add(i);
            }
            else
            {
                isEveryChoiceDisabled = false;
            }
        }
        disabledChoices = trimmedDisabledChoices;
        this.isEveryChoiceDisabled = isEveryChoiceDisabled;
    }

    public int GetNextChoice()
    {
        if (isEveryChoiceDisabled)
        {
            return -1;
        }
        do
        {
            currentChoice = (currentChoice + 1 - choiceLowerBound).Modulus(length) + choiceLowerBound;
        } while (disabledChoices.Contains(currentChoice));
        return currentChoice;
    }

    public int GetPreviousChoice()
    {
        if (isEveryChoiceDisabled)
        {
            return -1;
        }
        do
        {
            currentChoice = (currentChoice - 1 - choiceLowerBound).Modulus(length) + choiceLowerBound;
        } while (disabledChoices.Contains(currentChoice));
        return currentChoice;
    }

    public bool IsDisabled(int choice)
    {
        return disabledChoices.Contains(choice);
    }
}