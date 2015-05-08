using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace guiWords
{
    public class qHistory
    {
        String query;
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
        public qHistory()
        {

        }
        public qHistory(string s)
        {
            query = s;
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
    }
}
