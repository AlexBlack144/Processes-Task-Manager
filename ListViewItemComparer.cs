using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HWDispatcher
{
    internal class ListViewItemComparer : IComparer
    {
        private int _columIndex;
        private SortOrder _sortDirection;
        
        public int ColumIndex 
        {
            get { return _columIndex; }
            set { _columIndex = value; }
        }

        public SortOrder SortDirection
        {
            get { return _sortDirection; }
            set { _sortDirection = value; }
        }

        public ListViewItemComparer()
        {
            _sortDirection = SortOrder.None;
        }

        public int Compare(object x, object y)
        {
            ListViewItem listViewItemX = x as ListViewItem;
            ListViewItem listViewItemY = y as ListViewItem;

            int result;

            switch (_columIndex)
            {
                case 0:

                    result = string.Compare(listViewItemX.SubItems[_columIndex].Text,
                        listViewItemY.SubItems[_columIndex].Text, false);
                    
                        break;
                case 1:

                    double valueX = double.Parse(listViewItemX.SubItems[_columIndex].Text);
                    double valueY = double.Parse(listViewItemY.SubItems[_columIndex].Text);

                    result = valueX.CompareTo(valueY);

                    break;
                default:

                    result = string.Compare(listViewItemX.SubItems[_columIndex].Text,
                        listViewItemY.SubItems[_columIndex].Text, false);

                    break;
            }
            if (_sortDirection == SortOrder.Descending)
            {
                return -result;
            }
            else 
            {
                return result; 
            }
        }
    }
}
