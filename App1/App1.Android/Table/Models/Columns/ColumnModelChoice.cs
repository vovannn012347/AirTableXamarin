using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Models.Cells;
using App1.Droid.Table.Views;
using Firebase.Database;

namespace App1.Droid.Table.Models.Columns
{
    class ColumnModelChoice : ColumnModel, IDisposable
    {
        private ChoiceUpdate updateListener;

        private List<string> choices;
            
        private ArrayAdapter<String> choicesAdapter;
        
        public ColumnModelChoice() : base()
        {
            choices = new List<string>();
            choices.Add("");
            updateListener = new ChoiceUpdate(this);
            controller = new ColumnController(this);
        }

        public ColumnModelChoice(DataSnapshot data) : base(data)
        {
            choices = new List<string>();
            choices.Add("");
            foreach (String value in this.data.Values)
            {
                choices.Add(value);
            }

            updateListener = new ChoiceUpdate(this);
            controller = new ColumnController(this);
            

            data.Child("data").Ref.AddChildEventListener(updateListener);
        }
        
        public ArrayAdapter<String> getChoicesAdapter(Activity context)
        {
            if (choicesAdapter == null)
            {
                choicesAdapter = new ArrayAdapter<String>(context, Android.Resource.Layout.SimpleListItem1, choices);
            }
            return choicesAdapter;
        }

        public String IndexOfChoice(String choice)
        {
            if (data.ContainsValue(choice))
            {
                return ("" + choices.IndexOf(choice));
            }
            else
            {
                int temp;
                if(int.TryParse(choice, out temp) && temp < choices.Count)
                {
                    return choice;
                }
                else
                {
                    return "0";
                }
            }
        }

        public void Dispose()
        {
            updateListener.Dispose();
        }

        public override CellModel constructCell()
        {
            return new CellModelChoice(this);
        }

        public override ColumnView GetView(Activity context)
        {
            ColumnView cv = new ColumnView(context, controller);

            return cv;
        }

        public List<String> getChoices()
        {
            return choices;
        }

        public void setChoices(List<String> choices)
        {
            this.choices = choices;
        }

        class ChoiceUpdate : Java.Lang.Object, IChildEventListener
        {
            ColumnModelChoice parent;

            public ChoiceUpdate(ColumnModelChoice parent)
            {
                this.parent = parent;
            }

            public void OnChildAdded(DataSnapshot dataSnapshot, String previousChildName)
            {
                if (!parent.data.ContainsKey(dataSnapshot.Key))
                {
                    parent.data.Add(dataSnapshot.Key, dataSnapshot.Value.ToString());
                    int addIndex = parent.choices.IndexOf(parent.data.GetValueOrDefault(previousChildName));

                    if (addIndex > 1)
                    {
                        parent.choices[addIndex] = dataSnapshot.Value.ToString();
                    }
                    else
                    {
                        parent.choices.Add(dataSnapshot.Value.ToString());
                    }

                    if (parent.choicesAdapter != null)
                    {
                        parent.choicesAdapter.NotifyDataSetChanged();
                    }
                }
            }


            public void OnChildChanged(DataSnapshot dataSnapshot, String previousChildName)
            {
                if (parent.data.ContainsKey(dataSnapshot.Key))
                {
                    parent.data.Add(dataSnapshot.Key, dataSnapshot.Value.ToString());
                    int addIndex = parent.choices.IndexOf(parent.data.GetValueOrDefault(dataSnapshot.Key));
                    if (addIndex > 0)
                    {
                        //parent.choicesAdapter.Insert(dataSnapshot.Value.ToString(), addIndex);
                        parent.choices[addIndex] = dataSnapshot.Value.ToString();
                    }

                    if (parent.choicesAdapter != null)
                    {
                        parent.choicesAdapter.NotifyDataSetChanged();
                    }
                }

            }


            public void OnChildRemoved(DataSnapshot dataSnapshot)
            {
                if (parent.data.ContainsKey(dataSnapshot.Key))
                {
                    parent.data.Remove(dataSnapshot.Key);
                    parent.choices.Remove(dataSnapshot.Value.ToString());

                    if (parent.choicesAdapter == null)
                    {
                        parent.choicesAdapter.NotifyDataSetChanged();
                    }
                }
            }

            public void OnChildMoved(DataSnapshot dataSnapshot, String s)
            {
                ///?who cares
            }

            public void OnCancelled(DatabaseError databaseError)
            {
                //maybe i should log this
            }

        }
    }
}