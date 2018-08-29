package md5449c8e6ff4b3f37e625c5c45f9821bd2;


public class CellViewDate_UploadChangesTimerTask
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
		mono.android.Runtime.register ("App1.Droid.Table.Views.Cells.CellViewDate+UploadChangesTimerTask, App1.Android", CellViewDate_UploadChangesTimerTask.class, __md_methods);
	}


	public CellViewDate_UploadChangesTimerTask ()
	{
		super ();
		if (getClass () == CellViewDate_UploadChangesTimerTask.class)
			mono.android.TypeManager.Activate ("App1.Droid.Table.Views.Cells.CellViewDate+UploadChangesTimerTask, App1.Android", "", this, new java.lang.Object[] {  });
	}

	public CellViewDate_UploadChangesTimerTask (md5449c8e6ff4b3f37e625c5c45f9821bd2.CellViewDate p0)
	{
		super ();
		if (getClass () == CellViewDate_UploadChangesTimerTask.class)
			mono.android.TypeManager.Activate ("App1.Droid.Table.Views.Cells.CellViewDate+UploadChangesTimerTask, App1.Android", "App1.Droid.Table.Views.Cells.CellViewDate, App1.Android", this, new java.lang.Object[] { p0 });
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
