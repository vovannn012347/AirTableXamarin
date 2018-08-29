package md59c6a2b19eaa1b9a44f329ca39b67e570;


public abstract class CellModel
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("App1.Droid.Table.Models.CellModel, App1.Android", CellModel.class, __md_methods);
	}


	public CellModel ()
	{
		super ();
		if (getClass () == CellModel.class)
			mono.android.TypeManager.Activate ("App1.Droid.Table.Models.CellModel, App1.Android", "", this, new java.lang.Object[] {  });
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
