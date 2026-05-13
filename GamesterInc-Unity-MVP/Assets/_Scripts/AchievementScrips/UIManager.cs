using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] RectTransform studentNamesContainer = null;
    [Header("UI Elements (Prefabs)")]
    [SerializeField] Button studentButtonPrefab = null;
    [SerializeField] StudentData studentDataPrefab = null;
    [SerializeField] Canvas studentCanvas = null; // Reference to the canvas you want to open

    [SerializeField] private Slider averageGradeSlider = null; // Slider for average grade
    [SerializeField] private TextMeshProUGUI averageGradeText = null; // Text for average grade
    [SerializeField] private Slider passRateSlider = null; // Slider for pass rate
    [SerializeField] private TextMeshProUGUI passRateText = null; // Text for pass rate

    [SerializeField] private TMP_Text lastCourseText;
    [SerializeField] private int lastCourse = 3;

    [SerializeField] private float spacing = 10f; // Spacing between buttons

    private List<Button> studentButtons = new List<Button>();
    private List<Student> studentsList = new List<Student>();

    void Start()
    {
        lastCourseText.text = $"Current Course: {lastCourse}";
        // Example: Add student names
        AddStudentNames();

        // Calculate and display average grade
        CalculateAndDisplayAverageGrade();

        // Calculate and display pass rate
        CalculateAndDisplayPassRate();
    }

    void AddStudentNames()
    {
        // Clear any existing student name buttons
        ClearStudentNames();

        // Access the array of Student scriptable objects
        Student[] students = Resources.LoadAll<Student>("Students");

        // Add students to the list
        studentsList.AddRange(students);

        // Loop through each student and add their name to the UI
        foreach (Student student in students)
        {
            AddStudentButton(student);
        }

        // After adding all student buttons, adjust the size of the container GameObject
        AdjustContainerSize();
    }

    void AddStudentButton(Student student)
    {
        // Create a new student button UI element from the prefab
        Button newStudentButton = Instantiate(studentButtonPrefab, studentNamesContainer);

        // Set the text of the UI element to the student's name
        newStudentButton.GetComponentInChildren<TextMeshProUGUI>().text = student.Name;

        // Calculate the position of the new button based on its index in the list
        float buttonHeight = ((RectTransform)newStudentButton.transform).rect.height;
        float yPos = -buttonHeight * studentButtons.Count - spacing * studentButtons.Count;

        // Set the anchored position of the button
        RectTransform rectTransform = newStudentButton.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0f, yPos);

        // Add a click listener to the button
        newStudentButton.onClick.AddListener(() => ShowStudentData(student));

        // Add the UI element to the list for future reference
        studentButtons.Add(newStudentButton);
    }

    void ClearStudentNames()
    {
        // Destroy existing student name button UI elements
        foreach (Button studentButton in studentButtons)
        {
            Destroy(studentButton.gameObject);
        }

        // Clear the list of student name button UI elements
        studentButtons.Clear();
    }

    void AdjustContainerSize()
    {
        // Calculate the total height required to fit all the buttons
        float totalHeight = CalculateTotalHeight();

        // Set the size of the container GameObject to match the total height
        RectTransform containerRectTransform = studentNamesContainer.GetComponent<RectTransform>();
        containerRectTransform.sizeDelta = new Vector2(containerRectTransform.sizeDelta.x, totalHeight);
    }

    float CalculateTotalHeight()
    {
        // Calculate the total height required to fit all the buttons
        float buttonHeight = studentButtonPrefab.GetComponent<RectTransform>().rect.height;
        float totalHeight = buttonHeight * studentButtons.Count + spacing * (studentButtons.Count - 1);

        return totalHeight;
    }

    void ShowStudentData(Student student)
    {
        // Open the canvas
        studentCanvas.gameObject.SetActive(true);

        // Find the Student_Information_Holder GameObject under studentCanvas
        Transform studentInfoHolder = studentCanvas.transform.Find("Student_Information_Holder");
        if (studentInfoHolder != null)
        {
            // Find the Student_Info_BG GameObject under Student_Information_Holder
            Transform studentInfoBG = studentInfoHolder.Find("Student_Info_BG");
            if (studentInfoBG != null)
            {
                Transform infoHolder = studentInfoBG.Find("InfoHolder");
                // Find the StudentNameText, StudentGradeText, StudentTriesText, CompletedText, and TimeText components under Student_Info_BG
                Transform studentNameText = infoHolder.Find("StudentNameText");
                Transform studentGradeText = infoHolder.Find("StudentGradeText");
                Transform studentTriesText = infoHolder.Find("StudentTriesText");
                Transform studentCompletedText = infoHolder.Find("CompletedText");
                Transform studentTimeText = infoHolder.Find("TimeText");
                Transform studentLastCourse = infoHolder.Find("LastCourseText");

                // Check if components were found
                if (studentNameText != null && studentGradeText != null && studentTriesText != null && studentCompletedText != null && studentTimeText != null)
                {
                    // Get the TextMeshPro components
                    TextMeshProUGUI nameText = studentNameText.GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI gradeText = studentGradeText.GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI triesText = studentTriesText.GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI completedText = studentCompletedText.GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI timeText = studentTimeText.GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI lastCourseText = studentLastCourse.GetComponent<TextMeshProUGUI>();

                    // Set the text for the student name
                    nameText.text = "Student Name: " + student.Name;

                    // Set the text for the student grade
                    gradeText.text = "Student Grade: " + student.Grade;

                    // Set the text for the student's number of tries
                    triesText.text = "Tries: " + student.Tries;

                    // Set the text for the student's completed status
                    completedText.text = "Times Completed: " + student.Completed;

                    // Set the text for the student's time
                    timeText.text = "Best Time: " + student.Time + " seconds";

                    lastCourseText.text = $"Progress: Course {student.lastCourse}";
                    lastCourseText.color = (student.lastCourse < lastCourse) ? Color.red : Color.black;

                    // Exit the method as the components were found and updated successfully
                    return;
                }
                else
                {
                    Debug.LogError("Failed to find one or more text components under Student_Info_BG.");
                }
            }
            else
            {
                Debug.LogError("Failed to find Student_Info_BG GameObject under Student_Information_Holder.");
            }
        }
        else
        {
            Debug.LogError("Failed to find Student_Information_Holder GameObject under studentCanvas.");
        }

        // Log an error if any required component was not found
        Debug.LogError("Failed to find required components for displaying student data.");
    }

    void CalculateAndDisplayAverageGrade()
    {
        if (studentsList.Count == 0)
        {
            Debug.LogWarning("No students found to calculate average grade.");
            return;
        }

        // Calculate total grade sum
        float totalGradeSum = 0f;
        foreach (Student student in studentsList)
        {
            totalGradeSum += student.Grade;
        }

        // Calculate average grade
        float averageGrade = totalGradeSum / studentsList.Count;

        // Set the value of the slider to the average grade
        averageGradeSlider.value = averageGrade;

        // Display average grade text
        averageGradeText.text = averageGrade.ToString("F2");
    }

    void CalculateAndDisplayPassRate()
    {
        if (studentsList.Count == 0)
        {
            Debug.LogWarning("No students found to calculate pass rate.");
            return;
        }

        // Count the number of students who passed (grade >= minimum passing grade)
        int passingStudentsCount = 0;
        foreach (Student student in studentsList)
        {
            if (student.Grade >= 55f) // Assuming 55 is the minimum passing grade
            {
                passingStudentsCount++;
            }
        }

        // Calculate pass rate
        float passRate = (float)passingStudentsCount / studentsList.Count;

        // Set the value of the slider to the pass rate percentage
        passRateSlider.value = passRate * 100f;

        // Display pass rate text
        passRateText.text = (passRate * 100f).ToString("F2") + "%";
    }

}
