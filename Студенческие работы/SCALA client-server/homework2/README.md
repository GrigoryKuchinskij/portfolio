## Домашнее задание #2

В следующем задании необходимо реализовать распределённое приложение (с обменом сообщениями посредством использования сокетов), которое будет выполнять вычисление числа π [методом Монте-Карло](https://ru.wikipedia.org/wiki/%D0%9C%D0%B5%D1%82%D0%BE%D0%B4_%D0%9C%D0%BE%D0%BD%D1%82%D0%B5-%D0%9A%D0%B0%D1%80%D0%BB%D0%BE).


### Суть метода

![Визуализация вычислений методом Монте-Карло](https://hsto.org/getpro/habr/post_images/011/ef8/989/011ef8989001df204b33142805371d9b.gif)

Суть расчета числа π заключается в том, что берётся квадрат со стороной `a = 2 * R`, в него вписывается круг радиусом `R`. Случайным образом ставятся точки внутри квадрата. Геометрически вероятность того, что точка попадет в пределы круга, равна отношению площадей круга и квадрата:

    P = S○ / S▢ = π * R ^ 2 / a ^ 2 = π * R ^ 2 / (2 * R) ^ 2 = π * R ^ 2 / 2 ^ 2 * R ^ 2 = π / 4

Таким образом, чем больше случайных точек будет проверено, тем точнее будет результат.


### Основное приложение (master)

Задачи:

- запуск отдельного дочернего приложения с указанными параметрами
- отображение таблицы результатов в виде Web-страницы


### Дочернее приложение (slave)

Задачи:

- вычисление числа π методом Монте-Карло с указанным в качестве аргумента числом точек
- отправка результата вычислений основному приложению


### Взаимодействие приложений

Между собой основное и дочерние приложения должны общаться посредством отправки сообщений друг к другу.
