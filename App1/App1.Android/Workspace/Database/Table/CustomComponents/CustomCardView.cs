using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;

namespace App1.Droid.Workspace.Database.Table.CustomComponents
{
    public class CustomCardView : CardView
    {
        public CustomCardView(Context context) : base(context)
        {
        }

        public CustomCardView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public CustomCardView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        protected CustomCardView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            return false;
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return true;
        }
    }
}