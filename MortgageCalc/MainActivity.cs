using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using Debug = System.Diagnostics.Debug;

namespace MortgageCalc
{
    [Activity(Label                = "@string/app_name", 
              Theme                = "@style/AppTheme", MainLauncher = true,
              WindowSoftInputMode  = Android.Views.SoftInput.StateAlwaysHidden,
              ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation)]
    public class MainActivity : AppCompatActivity
    {
        private static readonly string TAG = "MainActivity";
        EditText edtPrice;
        SeekBar seekBarPrice;
        TextView txtFirstPay;
        SeekBar seekBarFirstPay;
        TextView txtCredit;
        TextView txtYear;
        SeekBar seekBarYear;
        CheckBox checkBoxAddKap;
        Button buttonRun;
        TextView txtResult;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            edtPrice        = FindViewById<EditText>(Resource.Id.edtPrice);
            seekBarPrice    = FindViewById<SeekBar>(Resource.Id.seekBarPrice);
            txtFirstPay     = FindViewById<TextView>(Resource.Id.txtFirstPay);
            seekBarFirstPay = FindViewById<SeekBar>(Resource.Id.seekBarFirstPay);
            txtCredit       = FindViewById<TextView>(Resource.Id.txtCredit);
            txtYear         = FindViewById<TextView>(Resource.Id.txtYear);
            seekBarYear     = FindViewById<SeekBar>(Resource.Id.SeekBarYear);
            checkBoxAddKap  = FindViewById<CheckBox>(Resource.Id.checkBoxAddKap);
            buttonRun       = FindViewById<Button>(Resource.Id.buttonRun);
            txtResult       = FindViewById<TextView>(Resource.Id.txtResult);
            seekBarPrice.ProgressChanged += (o, e) =>
            { 
                edtPrice.Text  = seekBarPrice.Progress.ToString();
                txtCredit.Text = GetCredit().ToString("#0.00");
                txtResult.Text = String.Empty;
            };
            seekBarFirstPay.ProgressChanged += (o, e) =>
            { 
                txtFirstPay.Text = "Первоначальный взнос, %:" + seekBarFirstPay.Progress.ToString();
                txtCredit.Text   = GetCredit().ToString("#0.00");
                txtResult.Text   = String.Empty;
            };
            checkBoxAddKap.CheckedChange += (o, e) =>
            {
                txtCredit.Text = GetCredit().ToString("#0.00");
                txtResult.Text = String.Empty;
            };
            seekBarYear.ProgressChanged += (o, e) =>
            {
                txtYear.Text   = "Срок, лет: " + seekBarYear.Progress.ToString(); 
                txtResult.Text = String.Empty;
            };
            edtPrice.TextChanged += (o, e) =>
            {
                txtCredit.Text = txtResult.Text = String.Empty;
            };
            buttonRun.Click += (o, e) =>
            {
                int price = 0;
                if (int.TryParse(edtPrice.Text, out price))
                {
                    if (price > 15000) price = 15000;
                    seekBarPrice.Progress = price;
                    edtPrice.Text = price.ToString();
                }
                txtCredit.Text = GetCredit().ToString("#0.00");
                txtResult.Text = "Кредитная ставка, %:" + GetCreditRate().ToString("#0.00");
                txtResult.Text += "\r\nЕжемесячный платеж, руб.: " + GetPayment().ToString("#0.00");
            };
            // Начальные установки
            edtPrice.Text    = seekBarPrice.Progress.ToString();
            txtFirstPay.Text = "Первоначальный взнос, %: " + seekBarPrice.Progress.ToString();
            txtYear.Text     = "Срок, лет: " + seekBarYear.Progress.ToString();
            txtCredit.Text   = GetCredit().ToString("#0.00");
            txtResult.Text   = String.Empty;
        }
        // расчет ежемес. платежа в рублях
        decimal GetPayment()
        {
            decimal result = 0;
            decimal proc = (decimal)GetCreditRate() / 100;
            int number_of_months = seekBarYear.Progress * 12; // количество месяцев
            // если количество месяцев равно 0, то возвращаем весь кредит сразу
            if (number_of_months == 0) return GetCredit() * 1000;
            // общая сумма: кредит и проценты по кредиту в месяц
            result = (GetCredit() + GetCredit() * proc * seekBarYear.Progress) / number_of_months;

            return result * 1000;
        }
        // Расчет кредитной ставки в %%
        double GetCreditRate()
        {
           double result = 15.5;
           if (seekBarFirstPay.Progress < 50) return result; // если первоначальный взнос менее 50%

           result = 15.0; // если первоначальный взноос больше и равен 50%
           if (seekBarYear.Progress < 20) result = 12.5; // если меньше 20 лет
           if (seekBarYear.Progress < 10) result = 10.0; // если меньше 20 лет
           
           return result;
        }

        // Расчет величины кредита в тыс. руб
        decimal GetCredit()
        {
            // Вычисляем недостяющий процент от стоимости недвижимости
            decimal proc = 1 - (decimal)seekBarFirstPay.Progress / 100;
            // величина кредита
            decimal credit = seekBarPrice.Progress * proc;
            if (checkBoxAddKap.Checked)
            {
                // Уменьшаем кредит, если есть материнский капитал
                credit -= 450;
            }
            // если кредит стал отрицательный, то брать займ не нужно
            if (credit < 0) credit = 0;
            return credit;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            Debug.WriteLine(TAG, "Выполнено OnDestroy");
        }
        protected override void OnStop()
        {
            base.OnStop();
            Debug.WriteLine(TAG, "Выполнено OnStop");
        }
        protected override void OnStart()
        {
            base.OnStop();
            Debug.WriteLine(TAG, "Выполнено OnStart");
        }
        protected override void OnPause()
        {
            base.OnStop();
            Debug.WriteLine(TAG, "Выполнено OnPause");
        }
        protected override void OnResume()
        {
            base.OnStop();
            Debug.WriteLine(TAG, "Выполнено OnResume");
        }
        protected override void OnRestart()
        {
            base.OnStop();
            Debug.WriteLine(TAG, "Выполнено OnRestart");
        }
        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            Debug.WriteLine(TAG, "Выполнено OnSaveInstanceState");
        }
        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            Debug.WriteLine(TAG, "Выполнено OnRestoreInstanceState");
        }
    }
}