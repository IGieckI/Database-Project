using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Project.Entities
{
    public record Feedback(int FeedbackId, int Rating, string Text, string Username);
}
