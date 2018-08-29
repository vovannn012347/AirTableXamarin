package md5d8558fc3c0b77977353d9b905555235c;


public class TableView_UpdatenameTask
	extends java.util.TimerTask
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_run:()V:GetRunHandler\n" +
			"";
		mono.android.Runtime.register ("App1.Droid.Table.Views.TableView+UpdatenameTask, App1.Android", TableView_UpdatenameTask.class, __md_methods);
	}


	public TableView_UpdatenameTask ()
	{
		super ();
		if (getClass () == TableView_UpdatenameTask.class)
			mono.android.TypeManager.Activate ("App1.Droid.Table.Views.TableView+UpdatenameTask, App1.Android", "", this, new java.lang.Object[] {  });
	}

	public TableView_UpdatenameTask (md5d8558fc3c0b77977353d9b905555235c.TableView p0)
	{
		super ();
		if (getClass () == TableView_UpdatenameTask.class)
			mono.android.TypeManager.Activate ("App1.Droid.Table.Views.TableView+UpdatenameTask, App1.Android", "App1.Droid.Table.Views.TableView, App1.Android", this, new java.lang.Object[] { p0 });
	}


	public void run ()
	{
		n_run ();
	}

	private native void n_run ();

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
