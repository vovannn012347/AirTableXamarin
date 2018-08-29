package md5d8558fc3c0b77977353d9b905555235c;


public abstract class CellView
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("App1.Droid.Table.Views.CellView, App1.Android", CellView.class, __md_methods);
	}


	public CellView ()
	{
		super ();
		if (getClass () == CellView.class)
			mono.android.TypeManager.Activate ("App1.Droid.Table.Views.CellView, App1.Android", "", this, new java.lang.Object[] {  });
	}

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
