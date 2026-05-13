using UnityEngine;

[CreateAssetMenu(fileName = "New Student", menuName = "TeacherMenu/New Student")]
public class Student : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private float _grade; // New field for the student's grade
    [SerializeField] private float _tries;
    [SerializeField] private float _completed;
    [SerializeField] private float _time;
    [SerializeField] private int _lastCourse;

    public string Name { get { return _name; } }
    public float Grade { get { return _grade; } set { _grade = value; } } // Property for accessing and setting the grade
    public float Tries { get { return _tries; } set { _tries = value; } }
    public float Completed { get { return _completed; } set { _completed = value; } }
    public float Time { get { return _time; } set { _time = value; } }
    public int lastCourse { get { return _lastCourse; } set { _lastCourse = value; } }
}
