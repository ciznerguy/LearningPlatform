using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningClassLibary.Models
{

        public class QuestionExam
        {
            public int QuestionID { get; set; }
            public string QuestionText { get; set; }
            public string ExpectedOutput { get; set; }
            public int TopicID { get; set; }
            public int DifficultyLevel { get; set; }

            public QuestionExam(int questionID, string questionText, string expectedOutput, int topicID, int difficultyLevel)
            {
                QuestionID = questionID;
                QuestionText = questionText;
                ExpectedOutput = expectedOutput;
                TopicID = topicID;
                DifficultyLevel = difficultyLevel;
            }

            public override string ToString()
            {
                return $"{QuestionID}: {QuestionText} (Difficulty: {DifficultyLevel})";
            }
        }
    }


