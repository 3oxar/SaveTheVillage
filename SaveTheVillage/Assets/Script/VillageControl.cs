using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class VillageControl : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI WheatText;//текст пшеницы
    [SerializeField] TextMeshProUGUI PeasantText;//текст крестьян
    [SerializeField] TextMeshProUGUI WarText;//текст войнов
    [SerializeField] TextMeshProUGUI QuantiAttackText;//кол-во нападающих
    [SerializeField] TextMeshProUGUI VolumeAudioButtonText;//текст кнопки изменения звука


    [SerializeField] Image WheatTimerImage;//отображения таймера нового сбора урожая
    [SerializeField] Image EatingTimerImage;//отображения таймера приема пищи
    [SerializeField] Image AttackTimerImage;//отображения таймера нападения
    [SerializeField] Image PeasantTimerImage;//отображение таймера появление нового крестьянина
    [SerializeField] Image WarTimerImage;//отображение таймера появление нового война

    [SerializeField] Button PeasantButton;//кнопка нанять крестьянина
    [SerializeField] Button WarButton;//кнопка нанять война

    [SerializeField] GameObject endGamePanel;//панель проигрыша
    [SerializeField] GameObject winGamePanel;//панель победы
    [SerializeField] GameObject PausePanel;//панель паузы

    [SerializeField] TextMeshProUGUI[] FinalStatisticsText = new TextMeshProUGUI[4];//текст на экране где будем выводить статистику
    [SerializeField] AudioSource[] GameAudioSources = new AudioSource[3];  

    int peasant = 5, 
        war = 0,
        wheat = 15; //переменные хранящие ресурсы
    int totalPeople;//всего людей
    int attack = 0;//нападающие войны
    int nullAttacker = 0;//переменая для атаки без врагов


    float getWheatTime, //время для получения пшеницы
          getPeasantTime,//время для получения нового крестьянина
          getWarTime,//время для получения нового война
          eatingTime,//время приема пищи
          attackTime,//время нападения 
          attackTimeIncrease = 25;//время для нападения 

    bool hirePeasant = false;//переменая для отслеживание нажатия кнопки что бы нанять крестьянина
    bool hireWar = false;//переменая для отслеживание нажатия кнопки что бы нанять война
    bool endGame = false;//проверка на конец игры
    bool winGame = false;//проверка на победу
    bool pause = false;//проверка на паузу
    bool checkVolumeAudio = false;//проверка на вкл/выкл звука


    int[] finalStatistics = new int[4];//переменая для подсчета собраного урожая, нанятых крестья и войнов, кол-во волн

    // Start is called before the first frame update
    void Start()
    {
        WheatWrite();   //  
        PeasantWrite(); //
        WarWrite();     //вывод стартовых ресурсов

        AttackWrite();//вывод кол-во нападающих

        CheckButton();//проверяем хватает ли ресурсов в начале игры для найма кого-то
        
    }

    // Update is called once per frame
    void Update()
    {
       

        if (pause == false)
        {
            getWheatTime += Time.deltaTime;//считаем время для получения урожая
            WheatTimerImage.fillAmount = getWheatTime / 3;//отображаем таймер получения урожая

            if (getWheatTime >= 3)//проверяем прошло ли 5 секунд
            {
                wheat += 1 * peasant;//добавляем за каждого креастьянина 1 пшеницу
                finalStatistics[0] += 1 * peasant;//считаем сколько добыто пшеницы за все время игры
                CheckButton();//првоеряем хватает ли ресурсво после сбора рожая 
                CheckWinGame();//проверяем выполнены условия победы
                getWheatTime = 0;//обнуляем время полученя урожая
                GameAudioSources[0].Play();//воспроизводим звук сбора урожая
                WheatWrite();//вывод пшеницы
            }

            eatingTime += Time.deltaTime;//считаем врем до приема пищи
            EatingTimerImage.fillAmount = eatingTime / 12;//отображаем таймер приема пищи

            if (eatingTime >= 12)//проверяем прошло ли 12 секунд
            {
                totalPeople = peasant + war;//считаем всего людей в деревне
                wheat -= totalPeople;//отнимаем от пшеницы кол-во еды которую съели люди
                eatingTime = 0;//обнуляем время приема пищи
                GameAudioSources[0].Play();//воспроизводим звук приема пищи
                WheatWrite();//вывод пищи
            }

            attackTime += Time.deltaTime;//считаем врем до нападения
            AttackTimerImage.fillAmount = attackTime / attackTimeIncrease;//отображаем таймер до атаки

            if (attackTime >= attackTimeIncrease)//проверяем прошло ли 30 секунд
            {
                GameAudioSources[2].Play();//воспроизводим звук атаки
                if (war < attack)
                {
                    endGame = true;
                    pause = true;
                }
                else if(nullAttacker < 2)//проверяем условия, если меньше 3 то атака будет без войска
                {
                    nullAttacker++;//увеличиваем переменукю, что бы через несколько ходов началис ь набеги с врагами
                    attackTime = 0;//обнуляем время до нападения 
                    finalStatistics[3] += 1;
                }
                else
                {
                    finalStatistics[3] += 1;//считаем сколько вол было за все время игры
                    attackTimeIncrease *= 1.1f;//увеличиваем время до след атаки
                    war -= attack;//отнимаем от войнов погибшее кол-во войнов
                    attack += 1;//увеличиваем кол-во нападающих
                    attackTime = 0;//обнуляем время до нападения 
                    WarWrite();//выводи кол-во войнов
                    AttackWrite();
                }
            }


            if (hirePeasant == true)//если была нажата кнопка переменая становиться true
            {
                getPeasantTime += Time.deltaTime;//начинаем считать для получения нового крестьянина
                PeasantTimerImage.fillAmount = getPeasantTime / 6;//отображаем таймер получения нового крестьянина

                if (getPeasantTime >= 6)//проверяем прошло ли 6 секунд
                {
                    peasant++;//добавляем крестьянина
                    finalStatistics[1] += 1;//считаем сколько крестьян было нанято за все время игры
                    PeasantButton.interactable = true;//делаем кнопку активной
                    getPeasantTime = 0;//обнуляем время получения нового крестьянина
                    hirePeasant = false;//убираем создание нового крестьянина
                    PeasantTimerImage.fillAmount = 0;//обнуляем шкалу времени нового крестьянина
                    PeasantWrite();//вывод крестьянина 
                    CheckButton();//проверяем хватает ли ресурсов после завершения покупки крестьянина
                    GameAudioSources[1].Play();//воспроизводим звук получения крестьянина
                }
            }

            if (hireWar == true)//если была нажата кнопка переменая становиться true
            {
                getWarTime += Time.deltaTime;//начинаем считать для получения нового война
                WarTimerImage.fillAmount = getWarTime / 8;//отображаем таймер получения нового война

                if (getWarTime >= 8)//проверяем прошло ли 8 секунд
                {
                    war++;//добавляем война
                    finalStatistics[2] += 1;//считаем сколько войнов было нанято за все время игры
                    WarButton.interactable = true;//делаем кнопку активной
                    getWarTime = 0;//обнуляем время получения нового крестьянина
                    hireWar = false;//убираем создание нового крестьянина
                    WarTimerImage.fillAmount = 0;//обнуляем шкалу времени нового крестьянина
                    WarWrite();//вывод войнов
                    CheckWinGame();//проверяем выполнены условия победы
                    CheckButton();//проверяем хватает ли ресурсов после завершения покупки вайна
                    GameAudioSources[1].Play();//воспроизводим звук получения война
                }
            }
        }
        else if (winGame == true)
        {
            FinalStatistics();
            winGamePanel.SetActive(true);//включаем панель победы
        }
        else if(endGame == true) 
        {

            FinalStatistics();
            endGamePanel.SetActive(true);//включаем панель проигрыша
        }
    }


    /// <summary>
    /// Начинаем получения нового крестьянина
    /// </summary>
    public void HirePeasant()
    {
        if(wheat >= 10)//если пшеницы хватает
        {
            hirePeasant = true;//переключаем переменую на создание крестьянина
            PeasantButton.interactable = false;//делаем кнопку неактивной
            wheat -= 10;//отнимаем пшеницу потраченую на крестьянина
            CheckButton();//проверяем хватает ли ресурсво после после покупки крестьянина
            WheatWrite();//вывод пшеницы
        }
       
    }

    /// <summary>
    /// Начинаем получение нового война
    /// </summary>
    public void HireWar()
    {
        if (wheat >= 20)//если пшеницы хватает
        {
            hireWar = true;//переключаем переменую на создание война
            WarButton.interactable = false;//делаем кнопку неактивной
            wheat -= 20;//отнимаем пшеницу потраченую на война
            CheckButton();//проверяем хватает ли ресурсво после после покупки война
            WheatWrite();//вывод пшеницы

        }
    }
         
    /// <summary>
    /// сбрасываем значение до стартовых
    /// </summary>
    public void RestartGame()
    {
        peasant = 5;
        war = 0;
        wheat = 20;

        getWheatTime = 0;
        getPeasantTime = 0;
        getWarTime = 0;
        eatingTime = 0;
        attackTime = 0;
        attackTimeIncrease = 30;

        hirePeasant = false;
        hireWar = false;
        endGame = false;
        pause = false;
        endGamePanel.SetActive(false);
        winGamePanel.SetActive(false);

        PeasantTimerImage.fillAmount = 0;
        WarTimerImage.fillAmount = 0;

        PeasantButton.interactable = true;
        WarButton.interactable = true;

        WheatWrite();
        PeasantWrite();
        WarWrite();
        AttackWrite();
    }

    /// <summary>
    /// Проверяем хватает ли ресурсов на пакупку крестьянина или война 
    /// </summary>
    void CheckButton()
    {
        if (wheat < 10 || hirePeasant == true) PeasantButton.interactable = false;//
        else PeasantButton.interactable = true;            //проверяем кнопку для найма крестьяница

        if(wheat < 20 || hireWar == true) WarButton.interactable = false; //
        else WarButton.interactable = true;            //проверяем кнопку для найма война

    }

    /// <summary>
    /// проверяем условия для победы 
    /// </summary>
    void CheckWinGame()
    {
        if (war >= 10 && wheat >= 250)
        {
            winGame = true;
            pause = true;
        }
    }

    /// <summary>
    /// останавлевает или запускает игру
    /// </summary>
    public void PauseGame()
    {
        if (pause != true)
        {
            Time.timeScale = 0;//если пауза включена останавливаем время
            PausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;//если пауза выключина запускаем время
            PausePanel.SetActive(false);
        }

        pause = !pause;//изменяем значение паузы при нажатие кнопки
    }

    /// <summary>
    /// Включение или выключение звука
    /// </summary>
    public void VolumeAudio()
    {
        if (checkVolumeAudio != true)
        {
            AudioListener.volume = 0;//если звук выключен устанавливаем громкость на 0
            VolumeAudioButtonText.text = "Вкл звук";
        }
        else
        { 
            AudioListener.volume = 1;//если звук включен устанавливаем громкость на 1
            VolumeAudioButtonText.text = "Выкл звук";
        }


        checkVolumeAudio = !checkVolumeAudio;//изменяем значение вкл/выкл звука
    }

    void WheatWrite() => WheatText.text = wheat.ToString();//вывод пшеницы
    void PeasantWrite() => PeasantText.text = peasant.ToString();//вывод крестьян
    void WarWrite() => WarText.text = war.ToString();//вывод война
    void AttackWrite() => QuantiAttackText.text = attack.ToString();//вывод кол-во нападающих
    
    /// <summary>
    /// записываем значение общей статистики
    /// </summary>
    void FinalStatistics()
    {
        FinalStatisticsText[0].text = finalStatistics[0].ToString();
        FinalStatisticsText[1].text = finalStatistics[1].ToString();
        FinalStatisticsText[2].text = finalStatistics[2].ToString();
        FinalStatisticsText[3].text = finalStatistics[3].ToString();
       
    }
}


