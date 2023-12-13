using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDemo2.Custom.Helper.OpenAI
{
    public enum Role
    {
        System,
        User,
        Assistant,
        Function
    }
    public static class RoleExtensitons
    {
        public static string ToRoleString(this Role role) => role switch
        {
            Role.System => "system",
            Role.User => "user",
            Role.Assistant => "assistant",
            _ => "function",
        };
    }
}
