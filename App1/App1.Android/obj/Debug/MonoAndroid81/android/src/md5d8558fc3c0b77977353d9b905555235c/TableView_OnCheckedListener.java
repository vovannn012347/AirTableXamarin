package md5d8558fc3c0b77977353d9b905555235c;


public class TableView_OnCheckedListener
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.widget.CompoundButton.OnCheckedChangeListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCheckedChanged:(Landroid/widget/CompoundButton;Z)V:GetOnCheckedChanged_Landroid_widget_CompoundButton_ZHandler:Android.Widget.CompoundButton/IOnCheckedChangeListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("App1.Droid.Table.Views.TableView+OnCheckedListener, App1.Android", TableView_OnCheckedListener.class, __md_methods);
	}


	public TableView_OnCheckedListener ()
	{
		super ();
		if (getClass () == TableView_OnCheckedListener.class)
			mono.android.TypeManager.Activate ("App1.Droid.Table.Views.TableView+OnCheckedListener, App1.Android", "", this, new java.lang.Object[] {  });
	}

	public TableView_OnCheckedListener (md5d8558fc3c0b77977353d9b905555235c.TableView p0)
	{
		super ();
		if (getClass () == TableView_OnCheckedListener.class)
			mono.android.TypeManager.Activate ("App1.Droid.Table.Views.TableView+OnCheckedListener, App1.Android", "App1.Droid.Table.Views.TableView, App1.Android", this, new java.lang.Object[] { p0 });
	}


	public void onCheckedChanged (android.widget.CompoundButton p0, boolean p1)
	{
		n_onCheckedChanged (p0, p1);
	}

	private native void n_onCheckedChanged (android.widget.CompoundButton p0, boolean p1);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
