package md5e46d3446d6fe382b0cfeb51d840f899a;


public class CellModelNumber
	extends md59c6a2b19eaa1b9a44f329ca39b67e570.CellModel
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("App1.Droid.Table.Models.Cells.CellModelNumber, App1.Android", CellModelNumber.class, __md_methods);
	}


	public CellModelNumber ()
	{
		super ();
		if (getClass () == CellModelNumber.class)
			mono.android.TypeManager.Activate ("App1.Droid.Table.Models.Cells.CellModelNumber, App1.Android", "", this, new java.lang.Object[] {  });
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
