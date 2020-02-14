using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    [Header("Input fields")]
    public InputField ifId;
    public Dropdown ifHandedness;
    public Dropdown ifGender;
    public InputField ifAge;
    public InputField ifForearmLength;
    public InputField ifForearmMarkerDist;
    public InputField ifFingerLength;
    public InputField ifUpperArmLength;
    public InputField ifUpperArmMarkerDist;
    public InputField ifHeight;
    public InputField ifArmLength;
    public InputField ifRightShoulderMarkerDistX;
    public InputField ifRightShoulderMarkerDistY;


    [Header("Other")]
    [Tooltip("Next scene to be loaded")]
    public string experiment = "VRPointing";
    public string logPath = "Assets/Resources/Log";
    public Text error;
    public Text controllers;

    void Start() {
        string[] joysticknames = Input.GetJoystickNames();
        joysticknames = joysticknames.Where(x => !string.IsNullOrEmpty(x))
            .Select(x => Regex.Match(x, "OpenVR [^-]+- (Left|Right)"))
            .Select(x => x.Groups[1].Value)
            .ToArray();
        controllers.text = string.Format("Controllers: {0}", string.Join(", ", joysticknames));

        PlayerPrefs.DeleteAll();
        
        ifId.onEndEdit.AddListener(OnIdChange);
        ifHandedness.onValueChanged.AddListener(OnHandednessChange);
        ifGender.onValueChanged.AddListener(OnGenderChange);
        ifAge.onEndEdit.AddListener(OnAgeChange);
        ifForearmLength.onEndEdit.AddListener(OnForearmLengthChange);
        ifForearmMarkerDist.onEndEdit.AddListener(OnForearmMarkerDistChange);
        ifFingerLength.onEndEdit.AddListener(OnFingerLengthChange);
        ifUpperArmLength.onEndEdit.AddListener(OnUpperArmLengthChange);
        ifUpperArmMarkerDist.onEndEdit.AddListener(OnUpperArmMarkerDistChange);
        ifHeight.onEndEdit.AddListener(OnHeightChange);
        ifArmLength.onEndEdit.AddListener(OnArmLengthChange);
        ifRightShoulderMarkerDistX.onEndEdit.AddListener(OnRightShoulderMarkerDistXChange);
        ifRightShoulderMarkerDistY.onEndEdit.AddListener(OnRightShoulderMarkerDistYChange);

        Directory.CreateDirectory(logPath);
        int participantId = Directory.GetDirectories(logPath).Length + 1;

        ifId.text = participantId.ToString();
        ifId.onEndEdit.Invoke(ifId.text);
        ifHandedness.onValueChanged.Invoke(ifHandedness.value);
        ifGender.onValueChanged.Invoke(ifGender.value);
        ifAge.onValueChanged.Invoke(ifAge.text);
        ifForearmLength.onEndEdit.Invoke(ifForearmLength.text);
        ifForearmMarkerDist.onEndEdit.Invoke(ifForearmMarkerDist.text);
        ifFingerLength.onEndEdit.Invoke(ifFingerLength.text);
        ifUpperArmLength.onEndEdit.Invoke(ifUpperArmLength.text);
        ifUpperArmMarkerDist.onEndEdit.Invoke(ifUpperArmMarkerDist.text);
        ifHeight.onEndEdit.Invoke(ifHeight.text);
        ifArmLength.onEndEdit.Invoke(ifArmLength.text);
        ifRightShoulderMarkerDistY.onEndEdit.Invoke(ifRightShoulderMarkerDistX.text);
        ifRightShoulderMarkerDistX.onEndEdit.Invoke(ifRightShoulderMarkerDistY.text);
    }

    private void OnIdChange(string v) { PlayerPrefs.SetInt("ParticipantId", int.Parse(v)); }
    private void OnHandednessChange(int v) { PlayerPrefs.SetInt("Handedness", v); }
    private void OnGenderChange(int v) { PlayerPrefs.SetInt("Gender", v); }
    private void OnAgeChange(string v) { PlayerPrefs.SetFloat("Age", OnFloatChange(v)); }
    private void OnForearmLengthChange(string v) { PlayerPrefs.SetFloat("ForearmLength", OnFloatChange(v)); }
    private void OnForearmMarkerDistChange(string v) { PlayerPrefs.SetFloat("ForearmMarkerDist", OnFloatChange(v)); }
    private void OnFingerLengthChange(string v) { PlayerPrefs.SetFloat("IndexfingerLength", OnFloatChange(v)); }
    private void OnUpperArmLengthChange(string v) { PlayerPrefs.SetFloat("UpperarmLength", OnFloatChange(v)); }
    private void OnUpperArmMarkerDistChange(string v) { PlayerPrefs.SetFloat("UpperarmMarkerDist", OnFloatChange(v)); }
    private void OnHeightChange(string v) { PlayerPrefs.SetFloat("Height", OnFloatChange(v)); }
    private void OnArmLengthChange(string v) { PlayerPrefs.SetFloat("ArmLength", OnFloatChange(v)); }
    private void OnRightShoulderMarkerDistXChange(string v) { PlayerPrefs.SetFloat("RightShoulderMarkerDist.X", OnFloatChange(v)); }
    private void OnRightShoulderMarkerDistYChange(string v) { PlayerPrefs.SetFloat("RightShoulderMarkerDist.Y", OnFloatChange(v)); }

    public float OnFloatChange(string v) { return float.Parse(v); }

    public void StartExperiment() {
        string[] measures = {
            "ParticipantId", "Handedness", "Gender", "Age",
            "IndexfingerLength","Height", "ArmLength",
            "UpperarmLength","UpperarmMarkerDist",
            "ForearmLength","ForearmMarkerDist",
            "RightShoulderMarkerDist.X", "RightShoulderMarkerDist.Y"
        };
        List<string> failed = new List<string>();

        foreach (string measure in measures) {
            if(!PlayerPrefs.HasKey(measure)) { failed.Add(measure); }
        }
        
        if(failed.Count > 0) {
            error.text = string.Format("validation error in {0}", string.Join(", ", failed));
            error.enabled = true;
            return;
        }

        Debug.LogFormat("StartScreen id: {0}", PlayerPrefs.GetInt("ParticipantId"));
        PlayerPrefs.Save();
        SceneManager.LoadScene(experiment);
    }
}
