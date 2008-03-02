using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;


namespace TheSign
{
    class ActionComboBoxClass
    {

        public delegate void OnSelectEvent();
        public class ActionItem
        {

            public OnSelectEvent onselectevent;
            public string text;
            public bool enabled;
            override public string ToString() 
            {
                return text;
            }
        }
        private Hashtable Actions = new Hashtable();
        private System.Windows.Forms.ComboBox myCombobox;
        private System.Windows.Forms.Control myControl;
        const string emptyText ="Nothing to do...";

        public ActionComboBoxClass(System.Windows.Forms.ComboBox cbox, System.Windows.Forms.Control okbutton)
        {
            myCombobox = cbox;
            myControl = okbutton;
            cbox.Items.Clear();
            myControl.Enabled = false;
            myCombobox.Text = emptyText;
        }
        public void addAction(string actionname, string actiontext, OnSelectEvent onselectevent)
        {
            ActionItem thisaction = new ActionItem();
            thisaction.onselectevent = onselectevent;
            thisaction.text = actiontext;
            thisaction.enabled = false;
            Actions[actionname] = thisaction;
        }
        public void update()
        {
            myCombobox.Items.Clear();
            foreach (ActionItem thisaction in Actions.Values)
            {
                if (thisaction.enabled)
                {
                    int i = myCombobox.Items.Add(thisaction);

                }
            }
            myControl.Enabled = myCombobox.Items.Count > 0;
            if (myCombobox.Items.Count > 0)
            {
                myCombobox.SelectedIndex = 0;
            }
            else
            {
                myCombobox.Text = emptyText;
            }
        }

        public void enableAction(string actionname)
        {
            try
            {
                ActionItem thisaction = (ActionItem)Actions[actionname];
                thisaction.enabled = true;
                update();
            }
            catch
            {
            }
        }

        public void disableAction(string actionname)
        {
            try
            {
                ActionItem thisaction = (ActionItem)Actions[actionname];
                thisaction.enabled = false;
                update();
            }
            catch
            {
            }
        }

    }
}
