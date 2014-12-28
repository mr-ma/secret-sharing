using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSharing.Lib.SharePart
{
    public class ShareCollection
    {
        public ShareCollection()
        {
            index = new List<IShare>();
        }
        private List<IShare> index;
        public IShare this[int i]
        {
            get
            {
                return index[i];
            }
            set
            {
                if (index.Count <= i)
                {
                    index.Add(value);
                }
                else
                {
                    index.Insert(i, value);
                }
            }
        }

        public static void ScatterShareIntoCollection(List<IShare> shares, ref List<ShareCollection> currentCollection, int index)
        {
            if (currentCollection.Count == 0)
            {
                for (int i = 0; i < shares.Count; i++)
                {
                    currentCollection.Add(new ShareCollection());
                }
            }

            for (int j = 0; j < shares.Count; j++)
            {
                currentCollection[j][index] = shares[j];
            }

        }
        public static List<IShare> GatherShareFromCollection(List<ShareCollection> currentCollection, int i)
        {
            List<IShare> shares = new List<IShare>();
            for (int j = 0; j < currentCollection.Count; j++)
            {
                shares.Add(currentCollection[j][i]);
            }
            return shares;
        }

        ~ShareCollection()
        {
            index.Clear();
            index = null;
        }

        public int Count
        {
            get
            {
                return index.Count;
            }
        }
    }
}
