using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.UI;

public class VillageControl : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI WheatText;//текст пшеницы
    [SerializeField] TextMeshProUGUI PeasantText;//текст крестьян
    [SerializeField] TextMeshProUGUI WarText;//текст войнов

    [SerializeField] TextMeshProUGUI QuantiAttackText;//кол-во нападающих


    [SerializeField] Image WheatTimerImage;//отображения таймера нового сбора урожая
    [SerializeField] Image EatingTimerImage;//отображения таймера приема пищи
    [SerializeField] Image AttackTimerImage;//отображения таймера нападения
    [SerializeField] Image PeasantTimerImage;//отображение таймера появление нового крестьянина
    [SerializeField] Image WarTimerImage;//отображение таймера появление нового война


    [SerializeField] Button PeasantButton;//кнопка нанять крестьянина
    [SerializeField] Button WarButton;//кнопка нанять война


    [SerializeField] GameObject endGamePanel;//панель проигрыша
    [SerializeField] GameObject winGamePanel;//панель победы

    int peasant = 5, 
        war = 0,
        wheat = 45; //переменные хранящие ресурсы
    int totalPeople;//всего людей
    int attack = 1;//нападающие войны
    


    float getWheatTime, //время для получения пшеницы
          getPeasantTime,//время для получения нового крестьянина
          getWarTime,//время для получения нового война
          eatingTime,//время приема пищи
          attackTime,//время нападения 
          attackTimeIncrease = 3;//время для нападения 

    bool hirePeasant = false;//переменая для отслеживание нажатия кнопки что бы нанять крестьянина
    bool hireWar = false;//переменая для отслеживание нажатия кнопки что бы нанять война
    bool endGame = false;//проверка на конец игры
    bool winGame = false;//проверка на победу
    bool pause = false;
    

    // Start is called before the first frame update
    void Start()
    {
        WheatWrite();   //  
        PeasantWrite(); //
        WarWrite();     //вывод стартовых ресурсов

        AttackWrite();//вывод кол-во нападающих
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

                getWheatTime = 0;//обнуляем время полученя урожая

                WheatWrite();//вывод пшеницы
            }

            eatingTime += Time.deltaTime;//считаем врем до приема пищи
            EatingTimerImage.fillAmount = eatingTime / 12;//отображаем таймер атаки

            if (eatingTime >= 12)//проверяем прошло ли 12 секунд
            {
                totalPeople = peasant + war;//считаем всего людей в деревне
                wheat -= totalPeople;//отнимаем от пшеницы кол-во еды которую съели люди
                eatingTime = 0;//обнуляем время приема пищи
                WheatWrite();//вывод пищи
            }

            attackTime += Time.deltaTime;//считаем врем до нападения
            AttackTimerImage.fillAmount = attackTime / attackTimeIncrease;//отображаем таймер до атаки

            if (attackTime >= attackTimeIncrease)//проверяем прошло ли 30 секунд
            {
                if (war < attack)
                {
                    endGame = true;
                    pause = true;
                }
                else
                {
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
                    PeasantButton.interactable = true;//делаем кнопку активной
                    getPeasantTime = 0;//обнуляем время получения нового крестьянина
                    hirePeasant = false;//убираем создание нового крестьянина
                    PeasantTimerImage.fillAmount = 0;//обнуляем шкалу времени нового крестьянина
                    PeasantWrite();//вывод крестьянина 
                }
            }

            if (hireWar == true)//если была нажата кнопка переменая становиться true
            {
                getWarTime += Time.deltaTime;//начинаем считать для получения нового война
                WarTimerImage.fillAmount = getWarTime / 8;//отображаем таймер получения нового война

                if (getWarTime >= 8)//проверяем прошло ли 8 секунд
                {
                    war++;//добавляем война
                    WarButton.interactable = true;//делаем кнопку активной
                    getWarTime = 0;//обнуляем время получения нового крестьянина
                    hireWar = false;//убираем создание нового крестьянина
                    WarTimerImage.fillAmount = 0;//обнуляем шкалу времени нового крестьянина
                    WarWrite();//вывод крестьянина 
                }
            }

            if (war >= 5 && wheat >= 100)
            {
                winGame = true;
                pause = true;
            }

        }
        else if (winGame == true)
        {
            winGamePanel.SetActive(true);//включаем панель победы
        }
        else if(endGame == true) 
        {
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

    void WheatWrite() => WheatText.text = wheat.ToString();//вывод пшеницы
    void PeasantWrite() => PeasantText.text = peasant.ToString();//вывод крестьян
    void WarWrite() => WarText.text = war.ToString();//вывод война
    void AttackWrite() => QuantiAttackText.text = attack.ToString();//вывод кол-во нападающих

}
