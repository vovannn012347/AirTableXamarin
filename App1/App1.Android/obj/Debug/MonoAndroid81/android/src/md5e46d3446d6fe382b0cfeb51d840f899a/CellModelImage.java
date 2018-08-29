package md5e46d3446d6fe382b0cfeb51d840f899a;


public class CellModelImage
	extends md59c6a2b19eaa1b9a44f329ca39b67e570.CellModel
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.tasks.OnSuccessListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onSuccess:(Ljava/lang/Object;)V:GetOnSuccess_Ljava_lang_Object_Handler:Android.Gms.Tasks.IOnSuccessListenerInvoker, Xamarin.GooglePlayServices.Tasks\n" +
			"";
		mono.android.Runtime.register ("App1.Droid.Table.Models.Cells.CellModelImage, App1.Android", CellModelImage.class, __md_methods);
	}


	public CellModelImage ()
	{
		super ();
		if (getClass () == CellModelImage.class)
			mono.android.TypeManager.Activate ("App1.Droid.Table.Models.Cells.CellModelImage, App1.Android", "", this, new java.lang.Object[] {  });
	}


	public void onSuccess (java.lang.Object p0)
	{
		n_onSuccess (p0);
	}

	private native void n_onSuccess (java.lang.Object p0);

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
