using Assets.HellionCat.SimpleExport;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class SampleFormCSV : MonoBehaviour {
    #region Fields
    [SerializeField]
    private Text m_scoreValue;
    [SerializeField]
    private Text m_emailInput;
    [SerializeField]
    private Text m_InfoLabel;
    [SerializeField]
    private ScoreCSV m_score;
    private Regex m_emailExpression;
    #endregion

    #region Methods
    // Use this for initialization
    void Start () {
		this.m_emailExpression = new Regex(@"^[a-zA-Z0-9][a-zA-Z0-9_-_.]+@[a-zA-Z0-9_-]+.[a-z]{2,}$");
    }
	
	// Update is called once per frame
	void Update () {
        this.m_scoreValue.text = m_score.score.ToString();
	}

    // Save the score and the email in a text file
    public void Save()
    {
        if (!this.m_emailExpression.IsMatch(this.m_emailInput.text))
        if (!this.m_emailExpression.IsMatch(this.m_emailInput.text))
        {
            this.m_InfoLabel.text = string.Format("Invalid Email");
            return;
        }
        if (SimpleExport_ScoreCSV.ExportCSV(m_score.score, m_emailInput.text)) {
            this.m_InfoLabel.text = string.Format("File saved {0}",DateTime.Now);
        }
        else
        {
            this.m_InfoLabel.text = string.Format("An error occured");
            return;
        }
    }
    #endregion
}
