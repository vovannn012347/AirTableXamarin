package md5449c8e6ff4b3f37e625c5c45f9821bd2;


public class CellViewDate
	extends md5d8558fc3c0b77977353d9b905555235c.CellView
	implements
		mono.android.IGCUserPeer,
		android.view.View.OnClickListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onClick:(Landroid/view/View;)V:GetOnClick_Landroid_view_View_Handler:Android.Views.View/IOnClickListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("App1.Droid.Table.Views.Cells.CellViewDate, App1.Android", CellViewDate.class, __md_methods);
	}


	public CellViewDate ()
	{
		super ();
		if (getClass () == CellViewDate.class)
			mono.android.TypeManager.Activate ("App1.Droid.Table.Views.Cells.CellViewDate, App1.Android", "", this, new java.lang.Object[] {  });
	}


	public void onClick (android.view.View p0)
	{
		n_onClick (p0);
	}

	private native void n_onClick (android.view.View p0);

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
