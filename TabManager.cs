using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AdvancedWebBrowser
{
    public class TabNode
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public WebBrowser Browser { get; set; }
        public TabPage TabPage { get; set; }
        public TabNode Previous { get; set; }
        public TabNode Next { get; set; }

        // History implemented as a linked list per tab
        public LinkedList<string> History { get; private set; }
        public LinkedListNode<string> CurrentHistoryNode { get; private set; }

        public TabNode(string title, string url, WebBrowser browser, TabPage tabPage)
        {
            Title = title;
            Url = url;
            Browser = browser;
            TabPage = tabPage;
            History = new LinkedList<string>();
            CurrentHistoryNode = null;
        }

        public void Visit(string url)
        {
            // If we're not at the end of history, truncate forward history
            if (CurrentHistoryNode != null && CurrentHistoryNode.Next != null)
            {
                var node = CurrentHistoryNode.Next;
                while (node != null)
                {
                    var next = node.Next;
                    History.Remove(node);
                    node = next;
                }
            }

            History.AddLast(url);
            CurrentHistoryNode = History.Last;
            Url = url;
        }

        public string GoBack()
        {
            if (CurrentHistoryNode?.Previous != null)
            {
                CurrentHistoryNode = CurrentHistoryNode.Previous;
                Url = CurrentHistoryNode.Value;
                return Url;
            }
            return null;
        }

        public string GoForward()
        {
            if (CurrentHistoryNode?.Next != null)
            {
                CurrentHistoryNode = CurrentHistoryNode.Next;
                Url = CurrentHistoryNode.Value;
                return Url;
            }
            return null;
        }
    }

    public class TabManager
    {
        private TabNode head;
        private TabNode tail;
        private TabNode current;
        private int count;

        public TabNode CurrentTab => current;
        public int Count => count;

        public event EventHandler<TabEventArgs> TabChanged;
        public event EventHandler<TabEventArgs> TabAdded;
        public event EventHandler<TabEventArgs> TabRemoved;

        public TabNode AddTab(string title, string url, WebBrowser browser, TabPage tabPage)
        {
            var newNode = new TabNode(title, url, browser, tabPage);

            if (head == null)
            {
                head = newNode;
                tail = newNode;
            }
            else
            {
                tail.Next = newNode;
                newNode.Previous = tail;
                tail = newNode;
            }

            count++;
            current = newNode;

            TabAdded?.Invoke(this, new TabEventArgs(newNode));
            TabChanged?.Invoke(this, new TabEventArgs(newNode));

            return newNode;
        }

        public void RemoveTab(TabNode node)
        {
            if (node == null) return;

            // Update links
            if (node.Previous != null)
                node.Previous.Next = node.Next;
            else
                head = node.Next;

            if (node.Next != null)
                node.Next.Previous = node.Previous;
            else
                tail = node.Previous;

            // Update current if needed
            if (current == node)
                current = node.Previous ?? head;

            count--;

            TabRemoved?.Invoke(this, new TabEventArgs(node));
            if (current != null)
                TabChanged?.Invoke(this, new TabEventArgs(current));
        }

        public void SwitchToTab(TabNode node)
        {
            if (node != null && node != current)
            {
                current = node;
                TabChanged?.Invoke(this, new TabEventArgs(node));
            }
        }

        public void SwitchToNextTab()
        {
            if (current?.Next != null)
            {
                current = current.Next;
                TabChanged?.Invoke(this, new TabEventArgs(current));
            }
        }

        public void SwitchToPreviousTab()
        {
            if (current?.Previous != null)
            {
                current = current.Previous;
                TabChanged?.Invoke(this, new TabEventArgs(current));
            }
        }

        public List<TabNode> GetAllTabs()
        {
            var tabs = new List<TabNode>();
            var current = head;
            while (current != null)
            {
                tabs.Add(current);
                current = current.Next;
            }
            return tabs;
        }

        public TabNode FindByTabPage(TabPage tabPage)
        {
            var node = head;
            while (node != null)
            {
                if (node.TabPage == tabPage)
                    return node;
                node = node.Next;
            }
            return null;
        }
    }

    public class TabEventArgs : EventArgs
    {
        public TabNode Tab { get; }

        public TabEventArgs(TabNode tab)
        {
            Tab = tab;
        }
    }
}