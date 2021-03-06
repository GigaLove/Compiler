﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LexicalAnalyzer
{
    public class SentenseResult
    {
        private int lineIndex;              //预测分析的当前行
        private string stackStr;            //栈顶符号
        private string inputStr;            //输入带当前符号
        private string action;              //动作信息，匹配或者产生式调用

        public SentenseResult(int lineIndex, string stackStr, string inputStr, string action)
        {
            this.lineIndex = lineIndex;
            this.stackStr = stackStr;
            this.inputStr = inputStr;
            this.action = action;
        }

        public int LineIndex
        {
            get { return lineIndex; }
            set { lineIndex = value; }
        }

        public string StackStr
        {
            get { return stackStr; }
            set { stackStr = value; }
        }

        public string InputStr
        {
            get { return inputStr; }
            set { inputStr = value; }
        }

        public string Action
        {
            get { return action; }
            set { action = value; }
        }

        public override string ToString()
        {
            return lineIndex + "; " + stackStr + "; " + inputStr + "; " + action;
        }
    }
}
