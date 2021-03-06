﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace guiWords
{
    public class qHistory
    {
        string query;
#region Publics
        public List<int> dWordIDs = new List<int>();
        public List<int> allIDs = new List<int>();
        public List<string> dForms = new List<string>();
        public List<string> dWords = new List<string>();
        public List<string> dMeanings = new List<string>();
        public List<string> pPart = new List<string>();
        public List<string> pDecl = new List<string>();
        public List<string> pConj = new List<string>();
        public List<string> pPerson = new List<string>();
        public List<string> pCase = new List<string>();
        public List<string> pNumber = new List<string>();
        public List<string> pGender = new List<string>();
        public List<string> pTense = new List<string>();
        public List<string> pMood = new List<string>();
        public List<string> pVoice = new List<string>();
#endregion
#region Constructors
        public qHistory()
        {

        }
        public qHistory(string s)
        {
            query = s;
        }
#endregion
#region Medthods
        public override string ToString()
        {
            string ret = string.Join(", ", dWords);
            return ret;
        }
        public Boolean isContained(List<string> q)
        {
            for (int i = 0; i < q.Count(); i++)
            {
                if (dForms.Contains(q[i]))
                {
                    return true;
                }
            }
            return false;
        }
#endregion
    }
}
