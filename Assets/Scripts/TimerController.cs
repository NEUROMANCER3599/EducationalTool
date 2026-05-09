using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using System;
using System.Collections.Generic;
using UnityEngine.Serialization;
using System.Collections;

public class TimerController : MonoBehaviour
{
    // ตัวแปรสำหรับเชื่อมต่อ Animator เพื่อควบคุม Animation ต่างๆ
    public Animator animator;
    
    [Header("Display References")]
    // TextMeshProUGUI สำหรับแสดงผลเวลานับถอยหลังตัวเลขใหญ่
    public TextMeshProUGUI countdownDisplay; 
    
    // TextMeshProUGUI สำหรับแสดงผล H/M/S ในโหมดตั้งค่า
    public TextMeshProUGUI hourText;
    public TextMeshProUGUI minuteText;
    public TextMeshProUGUI secondText;

    [Header("Delimiter References (Optional)")]
    // ตัวคั่นเวลา (เช่น : ) สำหรับโหมดตั้งค่า
    public TextMeshProUGUI hourMinuteDelimiter; 
    public TextMeshProUGUI minuteSecondDelimiter;
    
    [Header("Button References")]
    // **เปลี่ยนกลับ:** ใช้ปุ่มเดียวสำหรับ Start/Stop/Pause
    public Button startStopButton;
    // Container สำหรับปุ่มเพิ่ม/ลดเวลาในโหมดตั้งค่า
    public GameObject settingButtonsContainer;

    [Header("Button Sprites")]
    // **เปลี่ยนกลับ:** นำ Sprite สำหรับ Pause กลับมาใช้
    public Sprite playSprite; 
    public Sprite pauseSprite;

    [Header("SFX")]
    public AudioClip TimesUpSFX;
    public AudioClip TickSFX;
    public EffectBehaviour TimesUpFX;


    // ตัวแปรสำหรับควบคุม Logic
    private float timeRemaining = 0f;
    private bool isRunning = false;
    private Image buttonImage; // ตัวแปรสำหรับเก็บ Image component ของปุ่ม
    private bool isSettingMode = true;

    // ตัวแปรสำหรับเก็บค่าเวลาที่ตั้งไว้
    private int hoursSet = 0;
    private int minutesSet = 0;
    private int secondsSet = 0;
    // ค่าคงที่สำหรับจำกัดค่าเวลา
    private const int MAX_VALUE = 59;
    private const int MAX_HOUR = 99;

    private Coroutine TickingCoroutine;
    void Start()
    {
        // ตั้งค่า Image component และ Sprite เริ่มต้น
        if (startStopButton != null)
        {
            buttonImage = startStopButton.GetComponent<Image>();
            if (buttonImage != null && playSprite != null)
            {
                buttonImage.sprite = playSprite;
            }
        }
        
        UpdateSettingDisplay(); 
        SetMode(true); // เริ่มในโหมดตั้งค่า
    }

   void Update()
    {
        if (isRunning && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateCountdownDisplay();

            // ตรวจสอบเมื่อเวลานับถอยหลังถึงศูนย์
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isRunning = false;

                AudioManager.Instance.PlaySfx(TimesUpSFX, 1f);
                TimesUpFX.PlayFX();
                ResetTimer();
                Debug.Log("Countdown Finished! Holding End Animation.");
                
                // 1. Animation: สั่งไปที่ End และค้างไว้
                if (animator != null)
                {
                    animator.SetBool("Idle", false);
                    animator.SetBool("Loop", false);
                    animator.SetBool("End", true); 
                }

                // 2. UI/ปุ่ม: เปลี่ยนปุ่มเป็น Play/Start
                if (buttonImage != null && playSprite != null)
                {
                    buttonImage.sprite = playSprite;
                }
                
                // 3. แสดง 00:00:00 เป็นค่าสุดท้าย
                UpdateCountdownDisplay(); 
                
                // **ไม่เรียก SetMode(true)** เพื่อให้ UI ตั้งค่าถูกซ่อน และแสดง 00:00:00 ค้างไว้
            }
        }
    }
    
    // ----------------------------------------------------------------------
    // ฟังก์ชันควบคุมโหมด (Setting vs. Countdown)
    // ----------------------------------------------------------------------
    private void SetMode(bool settingMode)
    {
        isSettingMode = settingMode;
        
        bool showSettings = isSettingMode;
        
        // ควบคุม Animation (เฉพาะเมื่อเข้าสู่โหมดตั้งค่า หรือ Pause)
        if(animator != null && isSettingMode)
        {
            animator.SetBool("Idle", true);
            animator.SetBool("End", false);
            animator.SetBool("Loop", false);
        }
        
        // ควบคุม UI ที่เกี่ยวข้องกับโหมดตั้งค่า (H/M/S และปุ่มเพิ่มลด)
        if (settingButtonsContainer != null) settingButtonsContainer.SetActive(showSettings);
        if (hourText != null) hourText.gameObject.SetActive(showSettings);
        if (minuteText != null) minuteText.gameObject.SetActive(showSettings);
        if (secondText != null) secondText.gameObject.SetActive(showSettings);
        if (hourMinuteDelimiter != null) hourMinuteDelimiter.gameObject.SetActive(showSettings);
        if (minuteSecondDelimiter != null) minuteSecondDelimiter.gameObject.SetActive(showSettings);
        
        // ควบคุม UI ของเวลานับถอยหลังตัวเลขใหญ่
        if (countdownDisplay != null)
        {
            countdownDisplay.gameObject.SetActive(!showSettings);
            
            if (showSettings)
            {
                countdownDisplay.text = ""; 
            }
        }
    }
    
    // ----------------------------------------------------------------------
    // ฟังก์ชันควบคุมปุ่ม START/PAUSE/RESUME (Toggle)
    // ----------------------------------------------------------------------
   public void ToggleTimer()
{

    // A. ถ้าเวลานับถอยหลังถึงศูนย์แล้ว (อยู่ในสถานะ End Animation)
    if (timeRemaining <= 0 && !isSettingMode)
    {
        // ตรวจสอบว่ามีการตั้งเวลาใหม่หรือไม่
        if (hoursSet == 0 && minutesSet == 0 && secondsSet == 0)
        {
            Debug.Log("Please set a time before starting.");
            return;
        }

        // 1. ตั้งเวลาเริ่มต้น
        SetInitialTime();
        
        // 2. ออกจากโหมดตั้งค่า (เพื่อซ่อน UI ตั้งค่า)
        SetMode(false); 
        
        // 3. Start Logic:
        isRunning = true;
        buttonImage.sprite = pauseSprite;
        TickingCoroutine = StartCoroutine(clockticking());

        // 4. ควบคุม Animator ให้กลับไป Loop
        if (animator != null)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Loop", true);
            animator.SetBool("End", false); 
        }

        AudioManager.Instance.StopAllSound();
        
        return; 
    }
    
    // B. Logic เดิมสำหรับการ Start ครั้งแรก หรือ Pause/Resume ปกติ

    if (isSettingMode)
    {
        // START: (จากโหมด Idle/ตั้งค่า)
        
        // 1. ตรวจสอบว่ามีการตั้งเวลาหรือไม่
        if (hoursSet == 0 && minutesSet == 0 && secondsSet == 0)
        {
            Debug.Log("Please set a time before starting.");
            return; 
        }
        
        SetMode(false); 
        SetInitialTime();
        isRunning = true;
        buttonImage.sprite = pauseSprite;
        TickingCoroutine = StartCoroutine(clockticking());

            // ควบคุม Animator ให้เป็น Loop
        if (animator != null)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Loop", true);
            animator.SetBool("End", false);
        }
    }
    else
    {
        // PAUSE/RESUME: (ขณะอยู่ใน Loop)
        isRunning = !isRunning;
        if (isRunning)
        {
            // RESUME:
            if(animator != null)
            {
                animator.SetBool("Loop", true);
                animator.SetBool("Idle", false);
            }
            buttonImage.sprite = pauseSprite;
        }
        else
        {
            // PAUSE:
            if(animator != null)
            {
                animator.SetBool("Idle", true);
                animator.SetBool("Loop", false);
            }
            buttonImage.sprite = playSprite;
        }
    }

    }

    IEnumerator clockticking()
    {
        yield return new WaitForSeconds(1f);

        AudioManager.Instance.PlaySfx(TickSFX,0.75f);

        if(TickingCoroutine != null)
            TickingCoroutine = StartCoroutine(clockticking());
    }

    // ----------------------------------------------------------------------
    // ฟังก์ชันสำหรับรีเซ็ต (เชื่อมต่อกับปุ่ม Reset)
    // ----------------------------------------------------------------------
    public void ResetTimer()
    {
        // รีเซ็ตค่าเวลา
        timeRemaining = 0f; 
        isRunning = false; 
        hoursSet = 0;
        minutesSet = 0;
        secondsSet = 0;

        if (TickingCoroutine != null)
        {
            StopCoroutine(TickingCoroutine);
            TickingCoroutine = null;
        }

        UpdateSettingDisplay(); 
        SetMode(true); // กลับไปสู่โหมดตั้งค่า (และเปลี่ยน Animation เป็น Idle)
        
        // อัปเดตรูปภาพบนปุ่ม Start/Stop ให้กลับไปเป็น Play
        if (buttonImage != null && playSprite != null)
        {
            buttonImage.sprite = playSprite;
        }
    }
    
    // ----------------------------------------------------------------------
    // ฟังก์ชันควบคุมค่าเวลา H/M/S (ไม่เปลี่ยนแปลง)
    // ----------------------------------------------------------------------
    private void ClampValue(ref int value, int change, int max)
    {
        value += change;
        if (value < 0) value = 0;
        if (value > max) value = max;
        UpdateSettingDisplay();
        
        // ถ้ามีการเปลี่ยนแปลงค่าในโหมดตั้งค่า ให้มั่นใจว่า isSettingMode เป็น true
        if (!isRunning)
        {
            isSettingMode = true; 
        }
    }
    
    public void IncrementHour() { ClampValue(ref hoursSet, 1, MAX_HOUR);}
    public void DecrementHour() { ClampValue(ref hoursSet, -1, MAX_HOUR);}
    public void IncrementMinute() { ClampValue(ref minutesSet, 1, MAX_VALUE);}
    public void DecrementMinute() { ClampValue(ref minutesSet, -1, MAX_VALUE);}
    public void IncrementSecond() { ClampValue(ref secondsSet, 1, MAX_VALUE);}
    public void DecrementSecond() { ClampValue(ref secondsSet, -1, MAX_VALUE);}
    
    private void SetInitialTime()
    {
        // แปลง H/M/S เป็นวินาทีรวม
        timeRemaining = (hoursSet * 3600f) + (minutesSet * 60f) + secondsSet;
        if (timeRemaining <= 0) timeRemaining = 1f;
    }
    
    private void UpdateSettingDisplay()
    {
        // แสดงผล H/M/S ในโหมดตั้งค่า
        if (hourText != null) hourText.text = hoursSet.ToString("00");
        if (minuteText != null) minuteText.text = minutesSet.ToString("00");
        if (secondText != null) secondText.text = secondsSet.ToString("00");
    }
    
    // ----------------------------------------------------------------------
    // ฟังก์ชันแสดงผลเวลานับถอยหลัง (ไม่เปลี่ยนแปลง)
    // ----------------------------------------------------------------------
    private void UpdateCountdownDisplay()
    {
        TimeSpan time = TimeSpan.FromSeconds(timeRemaining);
        
        if (timeRemaining > 0)
        {
            // แสดงผลในรูปแบบ HH:MM:SS
            countdownDisplay.text = time.ToString(@"hh\:mm\:ss");
        }
        else
        {
            countdownDisplay.text = "00:00:00";

            if(TickingCoroutine != null)
            {
                StopCoroutine(TickingCoroutine);
                TickingCoroutine = null;
            }
        }
    }

}