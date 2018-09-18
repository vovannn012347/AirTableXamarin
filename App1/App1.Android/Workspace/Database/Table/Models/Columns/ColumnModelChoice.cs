using System;
using System.Collections.Generic;

using Android.App;
using Android.Widget;
using App1.Droid.Table.Models.Cells;
using App1.Droid.Table.Views;
using App1.Droid.Workspace.Database.Table.Views.Columns;
using Firebase.Database;

namespace App1.Droid.Table.Models.Columns
{
    class ColumnModelChoice : ColumnModel
    {
        private ChoiceUpdate updateListener;
        DatabaseReference dataReference;

        private List<string> choices;
        private List<string> choiceKeys;

        private ArrayAdapter<string> choicesAdapter;
        int largestchoice;

        public ColumnModelChoice() : base()
        {
            choices = new List<string>{""};
            choiceKeys = new List<string>();

            updateListener = new ChoiceUpdate(this);
        }
        public ColumnModelChoice(DataSnapshot data) : base(data)
        {
            choices = new List<string>{""};
            choiceKeys = new List<string>();

            this.Data = this.data;
            this.data.Clear();

            updateListener = new ChoiceUpdate(this);

            dataReference = data.Child("data").Ref;
            dataReference.AddChildEventListener(updateListener);
        }
          /*
        new public void Dispose()
        {
            choicesAdapter.Dispose();
            updateListener.Dispose();
            base.Dispose();
        }*/
        
        public string IndexOfChoice(string choice)
        {
            if (data.ContainsValue(choice))
            {
                return ("" + choices.IndexOf(choice));
            }
            else
            {
                if (int.TryParse(choice, out int temp) && temp < choices.Count)
                {
                    return choice;
                }
                else
                {
                    return "0";
                }
            }
        }

        public override CellModel ConstructCell()
        {
            return new CellModelChoice(this);
        }

        public ArrayAdapter<string> GetChoicesAdapter(Activity context)
        {
            if (choicesAdapter == null)
            {
                choicesAdapter = new ArrayAdapter<string>(context, Android.Resource.Layout.SimpleListItem1, choices);
            }
            return choicesAdapter;
        }
        public List<string> GetChoices()
        {
            return choices;
        }

        public void SetChoices(List<string> choices)
        {
            this.choices = choices;
        }

        public override ColumnView GetEditView(Activity context)
        {
            return new ColumnViewChoice(context, controller);
        }

        public override void SaveData()
        {
            Col_Ref.Child("data").RemoveValue();
            for (int i = 0; i < choiceKeys.Count; ++i)
            {
                Col_Ref.Child("data").Child(choiceKeys[i]).SetValue(choices[i+1]);
            }
        }

        public override Dictionary<string, string> Data {
            get {
                Dictionary<string, string> ret = new Dictionary<string, string>();
                for(int i = 0; i < choiceKeys.Count; ++i)
                {
                    ret.Add(choiceKeys[i], choices[i + 1]);
                }
                return ret;
            }
            set
            {
                if(value != null)
                {
                    if (value.ContainsKey("operation"))
                    {
                        switch (value["operation"])
                        {
                            case "del":
                                if (choiceKeys.Contains(value["key"]))
                                {
                                    choices.RemoveAt(choiceKeys.IndexOf(value["key"])+1);
                                    NotifyChoiceRemoved(choiceKeys.IndexOf(value["key"]));
                                    choiceKeys.Remove(value["key"]);
                                }
                                break;
                            case "patch":
                                if (choiceKeys.Contains(value["key"]))
                                {
                                    choices[choiceKeys.IndexOf(value["key"])+1] = value["text"];
                                    NotifyChoiceChanged(value["key"], value["text"]);
                                }
                                break;
                            case "put":
                                /*choiceKeys.Add("" + largestchoice + 1);
                                choices.Add(value["text"]);
                                NotifyChoiceAdded(choiceKeys.Count - 1, value["text"]);*/
                                break;
                            case "new":
                                choiceKeys.Add("" + largestchoice + 1);
                                choices.Add(value["text"]);
                                NotifyNewChoiceAdd("" + largestchoice + 1, value["text"]);
                                ++largestchoice;
                                break;
                        }
                    }
                    else
                    {

                        choices.Clear();
                        choices.Add("");
                        choiceKeys.Clear();

                        foreach (string key in value.Keys)
                        {
                            choices.Add(value[key]);
                            choiceKeys.Add(key);
                        }
                        if(choicesAdapter!= null)
                        {
                        choicesAdapter.AddAll(choices);
                        choicesAdapter.NotifyDataSetChanged();

                        }

                        NotifyDataChanged();
                    }
                }
                
            }
        }

        private void NotifyNewChoiceAdd(string newkey, string name)
        {
            Dictionary<string, string> notifyObject = new Dictionary<string, string>
            {
                { "operation", "new" },
                { "key", "" + newkey },
                { "value", "" + name }
            };

            controller.NotifyDataChanged(notifyObject);
        }

        private void NotifyDataChanged()
        {
            Dictionary<string, string> notifyObject = new Dictionary<string, string>();

            for (int i = 0; i < choices.Count; ++i)
            {
                notifyObject.Add("" + i, choices[i]);
            }

            controller.NotifyDataChanged(notifyObject);
        }

        private void NotifyChoiceRemoved(int removed)
        {
            Dictionary<string, string> notifyObject = new Dictionary<string, string>
            {
                { "operation", "del" },
                { "index", "" + removed }
            };

            controller.NotifyDataChanged(notifyObject);
        }

        private void NotifyChoiceChanged(string key, string choice)
        {
            Dictionary<string, string> notifyObject = new Dictionary<string, string>
            {
                { "operation", "patch" },
                { "key", key },
                { "value", "" + choice }
            };

            controller.NotifyDataChanged(notifyObject);
        }

        private void NotifyChoiceAdded(int addIndex, string key, string choice)
        {
            Dictionary<string, string> notifyObject = new Dictionary<string, string>
            {
                { "operation", "put" },
                { "index", "" + addIndex },
                { "key", key },
                { "value", "" + choice }
            };

            controller.NotifyDataChanged(notifyObject);
        }

        class ChoiceUpdate : Java.Lang.Object, IChildEventListener
        {
            ColumnModelChoice parent;

            public ChoiceUpdate(ColumnModelChoice parent)
            {
                this.parent = parent;
            }

            public void OnChildAdded(DataSnapshot dataSnapshot, string previousChildName)
            {
                parent.largestchoice = Math.Max( int.Parse(dataSnapshot.Key), parent.largestchoice);
                
                if (!parent.choiceKeys.Contains(dataSnapshot.Key))
                {
                    string choice = dataSnapshot.Value.ToString();
                    
                    if (!parent.choiceKeys.Contains(dataSnapshot.Key))
                    {
                        int addIndex = -1;
                        if(previousChildName == null)
                        {
                            addIndex = 0;
                        }else
                        if (!string.IsNullOrEmpty(previousChildName))
                        {
                            addIndex = parent.choiceKeys.IndexOf(previousChildName);
                        }
                        if (addIndex > -1)
                        {
                            parent.choices.Insert(addIndex+1, choice);
                            parent.choiceKeys.Insert(addIndex, dataSnapshot.Key);
                        }
                        else
                        {
                            parent.choices.Add(choice);
                            parent.choiceKeys.Add(dataSnapshot.Key);
                            addIndex = parent.choiceKeys.Count;
                        }

                        if (parent.choicesAdapter != null)
                        {
                            parent.choicesAdapter.Clear();
                            parent.choicesAdapter.AddAll(parent.choices);
                            parent.choicesAdapter.NotifyDataSetChanged();
                        }


                        parent.NotifyChoiceAdded(addIndex, dataSnapshot.Key, choice);
                    }
                    else
                    {
                        parent.NotifyChoiceChanged(dataSnapshot.Key, choice);
                    }
                }
            }
            
            public void OnChildChanged(DataSnapshot dataSnapshot, string previousChildName)
            {
                if (parent.choiceKeys.Contains(dataSnapshot.Key))
                {
                    string choice = dataSnapshot.Value.ToString();

                    int changeIndex;

                    changeIndex = parent.choiceKeys.IndexOf(dataSnapshot.Key);
                    
                    if (changeIndex > -1)
                    {
                        parent.choices[changeIndex+1] = choice;
                    }
                    
                    if (parent.choicesAdapter != null)
                    {
                        parent.choicesAdapter.Clear();
                        parent.choicesAdapter.AddAll(parent.choices);
                        parent.choicesAdapter.NotifyDataSetChanged();
                    }

                    parent.NotifyChoiceChanged(dataSnapshot.Key, choice);
                }
            }
            
            public void OnChildRemoved(DataSnapshot dataSnapshot)
            {

                if (parent.choiceKeys.Contains(dataSnapshot.Key))
                {
                    int removed = parent.choiceKeys.IndexOf(dataSnapshot.Key);
                    parent.choices.RemoveAt(removed+1);
                    parent.choiceKeys.RemoveAt(removed);

                    if (parent.choicesAdapter != null)
                    {
                        parent.choicesAdapter.Clear();
                        parent.choicesAdapter.AddAll(parent.choices);
                        parent.choicesAdapter.NotifyDataSetChanged();
                    }

                    parent.NotifyChoiceRemoved(removed);
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