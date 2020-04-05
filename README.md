Задание #4 Развитие Publish/Subscribe. Шаблоны Post/Redirect/Get, Retry
Постановка задачи
Требуется предоставить пользователям новый функционал: оценивать текст, задаваемый пользователем, по определённому критерию. Критерием оценки является отношение количества гласных букв к количеству согласных (в будущем алгоритм оценки планируется заменить на более сложный).

Реализация.
Работа является развитием Задания #3

Меняется протокол взаимодействия с BackendApi - в запросе на регистрацию Задачи добавляется поле Data, которое содержит заданный пользователем текст на оценку. Также добавляется метод GetProcessingResult для получения результата оценки текста. На фронтенде появляется поле ввода указанного текста.

Компонент BackendApi по прежнему сохраняет принятую Задачу в БД Redis и публикует событие JobCreated, содержащее идентификатор Задачи и возвращает значение идентификатора в ответе на запрос. Идентификатор Задачи в нашем случае является идентификатором контекста - ContextID, так как объединяет все этапы обработки текста в один логический контекст.

Frontend
Компонент Frontend, отправив текст на обработку в BackendApi, перенаправляет пользователя с помощью HTTP-статуса 302 Found (https://en.wikipedia.org/wiki/HTTP_302), на страницу просмотра оценки текста TextDetails с указанием ContextId в параметрах (например по адресу http://localhost/text-details?jobId=ContextId_Value), где пользователю отобразится результат оценивания текста.

При загрузке страницы TextDetails на серверной части происходит обращение к методу GetProcessingResult службы BackendApi для получения значения вычисленной оценки.

Post/Redirect/Get
Таким образом, взаимодействие пользователя с веб-приложением можно условно разбить на три этапа:

Post - пользователь методом POST отправляет данные на сервер;
Redirect - сервер регистрирует задачу и возвращает пользователю адрес, где он может получить результат;
Get - пользователь методом GET получает результат обработки по указанному адресу.
Подробнее о шаблоне Post/Redirect/Get (или PRG)

https://en.wikipedia.org/wiki/Post/Redirect/Get

Новый компонент: Служба оценки текста, TextRankCalc
В приложение нужно добавить новый компонент: служба TextRankCalc.

Компонент представляет собой консольное приложение, запускаемое в виде отдельного процесса. При запуске компонент подписывается на события JobCreated в шине сообщений. Действия при получении сообщения JobCreated:

По идентификатору в сообщении извлекает из Redis-хранилища соответствующий текст.
Подсчитывает количество гласных и согласных букв в тексте.
Подсчитывает оценку текста в виде отношения количества гласных к количеству согласных букв.
Записывает полученную оценку в БД Redis.
Новый метод BackendApi: Получение результата обработки, GetProcessingResult
В API службы BackendApi добавляется метод GetProcessingResult.

При обработке запроса GetProcessingResult служба BackendApi должна получить из БД Redis вычисленную оценку (помним, что оценку в БД записывает компонент TextRankCalc).

Если к текущему моменту запись с оценкой ещё не присутствует в БД, то выполняется несколько попыток получения с коротким "засыпанием" между попытками. Если после всех попыток в БД по-прежнему отсутствует запись с оценкой, то считается, что обработка текста ещё в процессе.

В ответе на запрос GetProcessingResult должны возвращаться статус обработки: "Завершена", "В процессе", и значение оценки текста, если таковая имеется.

Подробнее о шаблоне    Retry

https://docs.microsoft.com/en-us/azure/architecture/patterns/retry