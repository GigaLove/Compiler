﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LexicalAnalyzer
{
    class Scanner
    {
        private List<string> noteList;
        private List<TokenResult> tokenResList;
        private List<Error> errorList;
        private List<Symbol> symbolList;
        private int memIndex;

        public Scanner()
        {
            noteList = new List<string>();
            tokenResList = new List<TokenResult>();
            errorList = new List<Error>();
            symbolList = new List<Symbol>();
            memIndex = 0;
        }

        public void analyzeCode(string programStr, out List<TokenResult> outTokenResList, out List<Error> outErrorList, out List<Symbol> outSymbolList)
        {
            int beginIndex = 0;
            int endIndex = 0;
            int state = 0;
            int codeIndex = 0;
            int lineIndex = 1;
            string str = "";

            string codeStr = programStr;
            this.recognationNote(codeStr);
            codeStr = this.cutNote(codeStr) + "$";

            while (codeIndex < codeStr.Length)
            {
                
                switch (state)
                {
                    case 0:
                        if (codeStr[codeIndex] == '\n')
                        {
                            lineIndex++;
                        }
                        if (isLetter_(codeStr[codeIndex]))
                        {
                            state = 1;
                        }
                        else if (codeStr[codeIndex] == '"')
                        {
                            state = 2;
                        }
                        else if (codeStr[codeIndex] == '\'')
                        {
                            state = 5;
                        }
                        else if (codeStr[codeIndex] == '0')
                        {
                            state = 8;
                        }
                        else if (isOne2Nine(codeStr[codeIndex]))
                        {
                            state = 14;
                        }
                        else if (isSingleChar(codeStr[codeIndex]) > 0)
                        {
                            add2TokenResList(codeStr[codeIndex].ToString(), "_", isSingleChar(codeStr[codeIndex]), "单界符", lineIndex);
                        }
                        else if (codeStr[codeIndex] == '<')
                        {
                            state = 21;
                        }
                        else if (codeStr[codeIndex] == '>')
                        {
                            state = 24;
                        }
                        else if (codeStr[codeIndex] == '=')
                        {
                            state = 27;
                        }
                        else if (codeStr[codeIndex] == '!')
                        {
                            state = 29;
                        }
                        else if (codeStr[codeIndex] == '|')
                        {
                            state = 31;
                        }
                        else if (codeStr[codeIndex] == '^')
                        {
                            state = 34;
                        }
                        else if (codeStr[codeIndex] == '&')
                        {
                            state = 36;
                        }
                        else if (codeStr[codeIndex] == '+')
                        {
                            state = 39;
                        }
                        else if (codeStr[codeIndex] == '-')
                        {
                            state = 42;
                        }
                        else if (codeStr[codeIndex] == '/')
                        {
                            state = 46;
                        }
                        else if (codeStr[codeIndex] == '*')
                        {
                            state = 48;
                        }
                        else if (codeStr[codeIndex] == '%')
                        {
                            state = 50;
                        }
                        beginIndex = codeIndex;
                        break;
                    case 1:
                        if (!isLetter_(codeStr[codeIndex]) && !isDigit(codeStr[codeIndex]))
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            int flag = isKeyWord(str);
                            if (flag < 0)
                            {
                                add2TokenResList("IDN", str, 256, "标识符", lineIndex);
                                add2SymbolList(str, lineIndex);
                            }
                            else
                            {
                                add2TokenResList(str, "_", flag, "关键字", lineIndex);
                            }
                        }
                        break;
                    case 2:
                        if (codeStr[codeIndex] == '\\')
                        {
                            state = 3;
                        }
                        else if (codeStr[codeIndex] == '\n')
                        {
                            state = 0;
                            endIndex = codeIndex - 1;
                            str = codeStr.Substring(beginIndex, endIndex - beginIndex + 1);
                            add2ErrorList("字符串常数不闭合", str, lineIndex);
                        }
                        else if (codeStr[codeIndex] == '"')
                        {
                            state = 4;
                        }
                        break;
                    case 3:
                        state = 2;
                        break;
                    case 4:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList("STR", str, 351, "字符串常量", lineIndex);
                        break;
                    case 5:
                        if (codeStr[codeIndex] == '\\')
                        {
                            state = 6;
                        }
                        else if (codeStr[codeIndex] == '\n')
                        {
                            state = 0;
                            endIndex = codeIndex - 1;
                            str = codeStr.Substring(beginIndex, endIndex - beginIndex + 1);
                            add2ErrorList("字符常数不闭合", str, lineIndex);
                        }
                        else if (codeStr[codeIndex] == '\'')
                        {
                            state = 7;
                        }
                        break;
                    case 6:
                        state = 5;
                        break;
                    case 7:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList("CHAR", str, 350, "字符型常量", lineIndex);
                        break;
                    case 8:
                        if (isOne2Seven(codeStr[codeIndex]))
                        {
                            state = 9;
                        }
                        else if (codeStr[codeIndex] == 'x')
                        {
                            state = 10;
                        }
                        else if (codeStr[codeIndex] == '.')
                        {
                            state = 12;
                        }
                        else
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            add2TokenResList("INT10", str, 346, "整型常量", lineIndex);
                        }
                        break;
                    case 9:
                        if (!isZero2Seven(codeStr[codeIndex]))
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            add2TokenResList("INT8", str, 348, "8进制整型常量", lineIndex);
                        }
                        break;
                    case 10:
                        if ((codeStr[codeIndex] >= '1' && codeStr[codeIndex] <= '9') || (codeStr[codeIndex] >= 'a' && codeStr[codeIndex] <= 'f'))
                        {
                            state = 11;
                        }
                        else
                        {
                            state = 0;
                            endIndex = codeIndex;
                            codeIndex--;
                            str = codeStr.Substring(beginIndex, endIndex - beginIndex + 1);
                            add2ErrorList("无效的16进制数", str, lineIndex);
                        }
                        break;
                    case 11:
                        if (!((codeStr[codeIndex] >= '0' && codeStr[codeIndex] <= '9') || (codeStr[codeIndex] >= 'a' && codeStr[codeIndex] <= 'f')))
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            add2TokenResList("INT16", str, 347, "16进制整型常量", lineIndex);
                        }
                        break;
                    case 12:
                        if (isDigit(codeStr[codeIndex]))
                        {
                            state = 13;
                        }
                        else
                        {
                            state = 0;
                            endIndex = codeIndex;
                            codeIndex--;
                            str = codeStr.Substring(beginIndex, endIndex - beginIndex + 1);
                            add2ErrorList("无效的浮点数", str, lineIndex);
                        }
                        break;
                    case 13:
                        if (!isDigit(codeStr[codeIndex]))
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            add2TokenResList("FlOAT", str, 349, "浮点型常量", lineIndex);
                        }
                        break;
                    case 14:
                        if (isDigit(codeStr[codeIndex]))
                        {
                            state = 15;
                        }
                        else if (codeStr[codeIndex] == '.')
                        {
                            state = 16;
                        }
                        else
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            add2TokenResList("INT10", str, 346, "整型常量", lineIndex);
                        }
                        break;
                    case 15:
                        if (codeStr[codeIndex] == '.')
                        {
                            state = 12;
                        }
                        else if (!isDigit(codeStr[codeIndex]))
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            add2TokenResList("INT10", str, 346, "整型常量", lineIndex);
                        }
                        break;
                    case 16:
                        if (isDigit(codeStr[codeIndex]))
                        {
                            state = 17;
                        }
                        else
                        {
                            state = 0;
                            endIndex = codeIndex;
                            codeIndex--;
                            str = codeStr.Substring(beginIndex, endIndex - beginIndex + 1);
                            add2ErrorList("无效的浮点数", str, lineIndex);
                        }
                        break;
                    case 17:
                        if (codeStr[codeIndex] == 'e')
                        {
                            state = 18;
                        }
                        else if (isDigit(codeStr[codeIndex]))
                        {
                            codeIndex++;
                            continue;
                        }
                        else
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            add2TokenResList("FLOAT", str, 349, "浮点型常量", lineIndex);
                        }
                        break;
                    case 18:
                        if (isDigit(codeStr[codeIndex]))
                        {
                            state = 20;
                        }
                        else if (codeStr[codeIndex] == '+' || codeStr[codeIndex] == '-')
                        {
                            state = 19;
                        }
                        else
                        {
                            state = 0;
                            endIndex = codeIndex;
                            codeIndex--;
                            str = codeStr.Substring(beginIndex, endIndex - beginIndex + 1);
                            add2ErrorList("无效的科学技术法", str, lineIndex);
                        }
                        break;
                    case 19:
                        if (isOne2Nine(codeStr[codeIndex]))
                        {
                            state = 20;
                        }
                        else
                        {
                            state = 0;
                            endIndex = codeIndex;
                            codeIndex--;
                            str = codeStr.Substring(beginIndex, endIndex - beginIndex + 1);
                            add2ErrorList("无效的科学计数法", str, lineIndex);
                        }
                        break;
                    case 20:
                        if (isDigit(codeStr[codeIndex]))
                        {
                            codeIndex++;
                            continue;
                        }
                        else
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            add2TokenResList("FLOAT", str, 346, "浮点数常量", lineIndex);
                        }
                        break;
                    case 21:
                        if (codeStr[codeIndex] == '<')
                        {
                            state = 22;
                        }
                        else if (codeStr[codeIndex] == '=')
                        {
                            state = 23;
                        }
                        else
                        {
                            //if (codeStr[codeIndex] == '\n')
                            //{
                            //    lineIndex--;
                            //}
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);                       
                            add2TokenResList(str, "_", 314, "运算符", lineIndex);
                        }
                        break;
                    case 22:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList(str, "_", 312, "运算符", lineIndex);
                        break;
                    case 23:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList(str, "_", 315, "运算符", lineIndex);
                        break;
                    case 24:
                        if (codeStr[codeIndex] == '>')
                        {
                            state = 25;
                        }
                        else if (codeStr[codeIndex] == '=')
                        {
                            state = 26;
                        }
                        else
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            add2TokenResList(str, "_", 316, "运算符", lineIndex);
                        }
                        break;
                    case 25:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList(str, "_", 313, "运算符", lineIndex);
                        break;
                    case 26:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList(str, "_", 317, "运算符", lineIndex);
                        break;
                    case 27:
                        if (codeStr[codeIndex] == '=')
                        {
                            state = 28;
                        }
                        else
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            add2TokenResList(str, "_", 318, "运算符", lineIndex);
                        }
                        break;
                    case 28:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList(str, "_", 319, "运算符", lineIndex);
                        break;
                    case 29:
                        if (codeStr[codeIndex] == '=')
                        {
                            state = 30;
                        }
                        else
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            add2TokenResList(str, "_", 341, "运算符", lineIndex);
                        }
                        break;
                    case 30:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList(str, "_", 342, "运算符", lineIndex);
                        break;
                    case 31:
                        if (codeStr[codeIndex] == '|')
                        {
                            state = 32;
                        }
                        else if (codeStr[codeIndex] == '=')
                        {
                            state = 33;
                        }
                        else
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            add2TokenResList(str, "_", 320, "运算符", lineIndex);
                            break;
                        }
                        break;
                    case 32:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList(str, "_", 321, "运算符", lineIndex);
                        break;
                    case 33:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList(str, "_", 322, "运算符", lineIndex);
                        break;
                    case 34:
                        if (codeStr[codeIndex] == '=')
                        {
                            state = 35;
                        }
                        else
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            add2TokenResList(str, "_", 323, "运算符", lineIndex);
                        }
                        break;
                    case 35:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList(str, "_", 324, "运算符", lineIndex);
                        break;
                    case 36:
                        if (codeStr[codeIndex] == '&')
                        {
                            state = 37;
                        }
                        else if (codeStr[codeIndex] == '=')
                        {
                            state = 38;
                        }
                        else
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            add2TokenResList(str, "_", 325, "运算符", lineIndex);
                        }
                        break;
                    case 37:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList(str, "_", 326, "运算符", lineIndex);
                        break;
                    case 38:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList(str, "_", 327, "运算符", lineIndex);
                        break;
                    case 39:
                        if (codeStr[codeIndex] == '+')
                        {
                            state = 40;
                        }
                        else if (codeStr[codeIndex] == '=')
                        {
                            state = 41;
                        }
                        else
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            add2TokenResList(str, "_", 330, "运算符", lineIndex);
                        }
                        break;
                    case 40:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList(str, "_", 331, "运算符", lineIndex);
                        break;
                    case 41:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList(str, "_", 332, "运算符", lineIndex);
                        break;
                    case 42:
                        if (codeStr[codeIndex] == '-')
                        {
                            state = 43;
                        }
                        else if (codeStr[codeIndex] == '=')
                        {
                            state = 44;
                        }
                        else if (codeStr[codeIndex] == '>')
                        {
                            state = 45;
                        }
                        else
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            add2TokenResList(str, "_", 333, "运算符", lineIndex);
                        }
                        break;
                    case 43:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList(str, "_", 334, "运算符", lineIndex);
                        break;
                    case 44:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList(str, "_", 335, "运算符", lineIndex);
                        break;
                    case 45:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList(str, "_", 336, "运算符", lineIndex);
                        break;
                    case 46:
                        if (codeStr[codeIndex] == '=')
                        {
                            state = 47;
                        }
                        else
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            add2TokenResList(str, "_", 337, "运算符", lineIndex);
                        }
                        break;
                    case 47:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList(str, "_", 338, "运算符", lineIndex);
                        break;
                    case 48:
                        if (codeStr[codeIndex] == '=')
                        {
                            state = 49;
                        }
                        else
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            add2TokenResList(str, "_", 339, "运算符", lineIndex);
                        }
                        break;
                    case 49:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList(str, "_", 340, "运算符", lineIndex);
                        break;
                    case 50:
                        if (codeStr[codeIndex] == '=')
                        {
                            state = 51;
                        }
                        else
                        {
                            retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                            add2TokenResList(str, "_", 328, "运算符", lineIndex);
                        }
                        break;
                    case 51:
                        retract(ref codeIndex, ref state, ref endIndex, ref str, codeStr, beginIndex);
                        add2TokenResList(str, "_", 329, "运算符", lineIndex);
                        break;                       
                }
                codeIndex++;

            }          
            outTokenResList = tokenResList;
            outErrorList = errorList;
            outSymbolList = symbolList;
        }

        /// <summary>
        /// 回退指针，复位状态，截取词素
        /// </summary>
        /// <param name="codeIndex">下标指针</param>
        /// <param name="state">状态</param>
        /// <param name="endIndex">词素的结束位置</param>
        /// <param name="str">待截取的词素</param>
        /// <param name="codeStr">源码</param>
        /// <param name="beginIndex">词素的开始位置</param>
        private void retract(ref int codeIndex, ref int state, ref int endIndex, ref string str, string codeStr, int beginIndex)
        {
            codeIndex--;
            state = 0;
            endIndex = codeIndex;
            str = codeStr.Substring(beginIndex, endIndex - beginIndex + 1);
        }

        /// <summary>
        /// 添加到tokenResList中
        /// </summary>
        /// <param name="tokenName"></param>
        /// <param name="tokenValue"></param>
        /// <param name="typeNum"></param>
        /// <param name="className"></param>
        /// <param name="lineIndex"></param>
        private void add2TokenResList(string tokenName, string tokenValue, int typeNum, string className, int lineIndex)
        {
            Token tk = new Token(tokenName, tokenValue);
            TokenResult tr = new TokenResult(tk, typeNum, className, lineIndex);
            tokenResList.Add(tr);
        }

        private void add2SymbolList(string symbolName, int lineIndex)
        {
            bool flag = false;
            foreach (Symbol symbol in symbolList)
            {
                if (symbolName.Equals(symbol.SymbolName))
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                symbolList.Add(new Symbol(symbolName, memIndex, lineIndex));
                this.memIndex++;
            }
        }

        private void add2ErrorList(string errorType, string errorWord, int errorLine)
        {
            errorList.Add(new Error(errorType, errorWord, errorLine));
        }

        /// <summary>
        /// 判断字符是否是1-9
        /// </summary>
        private bool isOne2Nine(char c)
        {
            bool flag = false;
            if (c >= '1' && c <= '9')
            {
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符是否是1-7
        /// </summary>
        private bool isOne2Seven(char c)
        {
            bool flag = false;
            if (c >= '1' && c <= '7')
            {
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符是否是0-7
        /// </summary>
        private bool isZero2Seven(char c)
        {
            bool flag = false;
            if (c >= '0' && c <= '7')
            {
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符是否为0-9
        /// </summary>
        private bool isDigit(char c)
        {
            bool flag = false;
            if (c >= '0' && c <= '9')
            {
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符是否为字母
        /// </summary>
        private bool isLetter(char c)
        {
            bool flag = false;
            if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
            {
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符是否是字母或者"_"
        /// </summary>
        private bool isLetter_(char c)
        {
            bool flag = false;
            if (isLetter(c) || c == '_')
            {
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 判断字符串是否是关键字
        /// </summary>
        /// <param name="keyWord">关键字字符串</param>
        /// <returns>匹配，关键字对应的种别码；否则，返回-1</returns>
        private int isKeyWord(string keyWord)
        {
            int flag = -1;
            for (int i = 0; i < ConstTable.keyWords.Length; i++)
            {
                if (keyWord.Equals(ConstTable.keyWords[i]))
                {
                    flag = 257 + i;
                }
            }
            return flag;
        }

        /// <summary>
        /// 判断字符是否是单界符
        /// </summary>
        /// <param name="c">单界符</param>
        /// <returns>匹配，返回单界符的种别码；否则，返回-1</returns>
        private int isSingleChar(char c)
        {
            int flag = -1;
            for (int i = 0; i < ConstTable.singleChars.Length; i++)
            {
                if (c == ConstTable.singleChars[i])
                {
                    flag = 299 + i;
                }
            }
            return flag;
        }

        /// <summary>
        /// 判断字符是否为特殊字符
        /// </summary>
        private bool isSkip(char c)
        {
            bool flag = false;
            if (c == '\n' || c == ' ' || c == '\r' || c == '\t')
            {
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 识别注释，并添加到noteList中
        /// </summary>
        /// <param name="codeStr">程序代码</param>
        private void recognationNote(string codeStr)
        {
            int stateIndex = 0;
            int beginIndex = 0;
            int endIndex = 0;

            for (int i = 0; i < codeStr.Length; i++)
            {
                switch (stateIndex)
                {
                    case 0:
                        if (codeStr[i] == '/')
                        {
                            stateIndex = 1;
                            beginIndex = i;
                        }
                        break;
                    case 1:
                        if (codeStr[i] == '*')
                        {
                            stateIndex = 2;
                        }
                        else if (codeStr[i] == '/')
                        {
                            stateIndex = 5;
                        }
                        break;
                    case 2:
                        if (codeStr[i] == '*')
                        {
                            stateIndex = 3;
                        }
                        break;
                    case 3:
                        if (codeStr[i] == '/')
                        {
                            stateIndex = 4;
                        }
                        else if (codeStr[i] != '*')
                        {
                            stateIndex = 2;
                        }
                        break;
                    case 4:
                        stateIndex = 0;
                        endIndex = i;
                        noteList.Add(codeStr.Substring(beginIndex, endIndex - beginIndex));
                        break;
                    case 5:
                        if (codeStr[i] == '\n')
                        {
                            stateIndex = 6;
                        }
                        break;
                    case 6:
                        stateIndex = 0;
                        endIndex = i - 1;
                        noteList.Add(codeStr.Substring(beginIndex, endIndex - beginIndex));
                        break;
                }
            }
            for (int i = 0; i < noteList.Count; i++)
            {
                Console.WriteLine(noteList[i]);
            }
        }

        /// <summary>
        /// 根据noteList中获取的注释，替换源代码
        /// </summary>
        /// <param name="codeStr">源程序代码</param>
        /// <returns>去除注释后的代码</returns>
        private string cutNote(string codeStr)
        {
            string replaceStr;

            for (int i = 0; i < noteList.Count; i++)
            {
                replaceStr = "";
                foreach (char c in noteList[i])
                {
                    if (c == '\n')
                    {
                        replaceStr += c.ToString();
                    }
                }
                codeStr = codeStr.Replace(noteList[i], replaceStr);
            }
            Console.Write("CodeStr:\n" + codeStr);
            return codeStr;
        }
    }
}