package md5a445f6db5abf8b5b69aa92721cad9446;


public class MainActivity
	extends md51558244f76c53b6aeda52c8a337f2c37.FormsAppCompatActivity
	implements
		mono.android.IGCUserPeer,
		android.support.design.widget.NavigationView.OnNavigationItemSelectedListener,
		com.google.firebase.database.ChildEventListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onDestroy:()V:GetOnDestroyHandler\n" +
			"n_onActivityResult:(IILandroid/content/Intent;)V:GetOnActivityResult_IILandroid_content_Intent_Handler\n" +
			"n_onNavigationItemSelected:(Landroid/view/MenuItem;)Z:GetOnNavigationItemSelected_Landroid_view_MenuItem_Handler:Android.Support.Design.Widget.NavigationView/IOnNavigationItemSelectedListenerInvoker, Xamarin.Android.Support.Design\n" +
			"n_onCancelled:(Lcom/google/firebase/database/DatabaseError;)V:GetOnCancelled_Lcom_google_firebase_database_DatabaseError_Handler:Firebase.Database.IChildEventListenerInvoker, Xamarin.Firebase.Database\n" +
			"n_onChildAdded:(Lcom/google/firebase/database/DataSnapshot;Ljava/lang/String;)V:GetOnChildAdded_Lcom_google_firebase_database_DataSnapshot_Ljava_lang_String_Handler:Firebase.Database.IChildEventListenerInvoker, Xamarin.Firebase.Database\n" +
			"n_onChildChanged:(Lcom/google/firebase/database/DataSnapshot;Ljava/lang/String;)V:GetOnChildChanged_Lcom_google_firebase_database_DataSnapshot_Ljava_lang_String_Handler:Firebase.Database.IChildEventListenerInvoker, Xamarin.Firebase.Database\n" +
			"n_onChildMoved:(Lcom/google/firebase/database/DataSnapshot;Ljava/lang/String;)V:GetOnChildMoved_Lcom_google_firebase_database_DataSnapshot_Ljava_lang_String_Handler:Firebase.Database.IChildEventListenerInvoker, Xamarin.Firebase.Database\n" +
			"n_onChildRemoved:(Lcom/google/firebase/database/DataSnapshot;)V:GetOnChildRemoved_Lcom_google_firebase_database_DataSnapshot_Handler:Firebase.Database.IChildEventListenerInvoker, Xamarin.Firebase.Database\n" +
			"";
		mono.android.Runtime.register ("App1.Droid.MainActivity, App1.Android", MainActivity.class, __md_methods);
	}


	public MainActivity ()
	{
		super ();
		if (getClass () == MainActivity.class)
			mono.android.TypeManager.Activate ("App1.Droid.MainActivity, App1.Android", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void onDestroy ()
	{
		n_onDestroy ();
	}

	private native void n_onDestroy ();


	public void onActivityResult (int p0, int p1, android.content.Intent p2)
	{
		n_onActivityResult (p0, p1, p2);
	}

	private native void n_onActivityResult (int p0, int p1, android.content.Intent p2);


	public boolean onNavigationItemSelected (android.view.MenuItem p0)
	{
		return n_onNavigationItemSelected (p0);
	}

	private native boolean n_onNavigationItemSelected (android.view.MenuItem p0);


	public void onCancelled (com.google.firebase.database.DatabaseError p0)
	{
		n_onCancelled (p0);
	}

	private native void n_onCancelled (com.google.firebase.database.DatabaseError p0);


	public void onChildAdded (com.google.firebase.database.DataSnapshot p0, java.lang.String p1)
	{
		n_onChildAdded (p0, p1);
	}

	private native void n_onChildAdded (com.google.firebase.database.DataSnapshot p0, java.lang.String p1);


	public void onChildChanged (com.google.firebase.database.DataSnapshot p0, java.lang.String p1)
	{
		n_onChildChanged (p0, p1);
	}

	private native void n_onChildChanged (com.google.firebase.database.DataSnapshot p0, java.lang.String p1);


	public void onChildMoved (com.google.firebase.database.DataSnapshot p0, java.lang.String p1)
	{
		n_onChildMoved (p0, p1);
	}

	private native void n_onChildMoved (com.google.firebase.database.DataSnapshot p0, java.lang.String p1);


	public void onChildRemoved (com.google.firebase.database.DataSnapshot p0)
	{
		n_onChildRemoved (p0);
	}

	private native void n_onChildRemoved (com.google.firebase.database.DataSnapshot p0);

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
